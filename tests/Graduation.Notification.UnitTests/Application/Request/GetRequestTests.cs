using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Graduation.Notification.Application.Request.GetRequest;
using Graduation.Notification.Application.Request.GetRequest.Boundaries;
using Graduation.Notification.Domain.Entities;
using Graduation.Notification.Domain.Exceptions;
using Graduation.Notification.Domain.Gateways.Logger;
using Graduation.Notification.Domain.Gateways.MemoryCache;
using Graduation.Notification.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Graduation.Notification.UnitTests.Application.Request;

public sealed class GetRequestTests
{
    private readonly ILoggerManager _logger;
    private readonly IMemoryCacheManager _memoryCache;
    private readonly IValidator<GetRequestInput> _validator;

    public GetRequestTests()
    {
        _logger = Substitute.For<ILoggerManager>();
        _memoryCache = Substitute.For<IMemoryCacheManager>();
        _validator = Substitute.For<IValidator<GetRequestInput>>();
    }

    [Fact]
    public void Interactor_ValidInput_ShouldReturnRequest()
    {
        //Arrange
        var requestId = Guid.NewGuid();
        var requestEntity = new RequestEntity
        {
            RequestId = requestId,
            Status = RequestStatus.NotStarted,
            Message = ResponseMessage.Default
        };

        _validator.ValidateAsync(Arg.Any<GetRequestInput>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(new ValidationResult()));
        _memoryCache.GetAsync<RequestEntity>(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                     .Returns(Task.FromResult(requestEntity));

        var sut = GenerateInteractor();

        //Act
        var output = sut.GetAsync(new GetRequestInput(requestId), CancellationToken.None).Result;

        //Assert
        output.Data.RequestId.Should().Be(requestId);
        output.Data.Status.Should().Be(requestEntity.Status);
        output.Data.Message.Should().Be(requestEntity.Message);
    }

    [Fact]
    public void Interactor_InvalidInput_ShouldThrow()
    {
        //Arrange
        var input = new GetRequestInput(Guid.NewGuid());

        _validator.ValidateAsync(Arg.Any<GetRequestInput>(), Arg.Any<CancellationToken>()).ThrowsAsync<BusinessException>();

        var sut = GenerateInteractor();

        //Act
        Func<Task> act = async () => await sut.GetAsync(input, CancellationToken.None);

        //Assert
        act.Should().ThrowAsync<BusinessException>();
    }

    [Theory]
    [InlineData(RequestStatus.NotStarted)]
    [InlineData(RequestStatus.Completed)]
    [InlineData(RequestStatus.Processing)]
    [InlineData(RequestStatus.Canceled)]
    [InlineData(RequestStatus.InvalidInformation)]
    [InlineData(RequestStatus.InfrastructureError)]

    public void Interactor_Return_Different_Statuses(RequestStatus status)
    {
        //Arrange
        var requestId = Guid.NewGuid();
        var requestEntity = new RequestEntity
        {
            RequestId = requestId,
            Status = status,
            Message = ResponseMessage.Default
        };

        _validator.ValidateAsync(Arg.Any<GetRequestInput>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(new ValidationResult()));
        _memoryCache.GetAsync<RequestEntity>(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                     .Returns(Task.FromResult(requestEntity));

        var sut = GenerateInteractor();

        //Act
        var output = sut.GetAsync(new GetRequestInput(requestId), CancellationToken.None).Result;

        //Assert
        output.Data.RequestId.Should().Be(requestId);
        output.Data.Status.Should().Be(requestEntity.Status);
        output.Data.Message.Should().Be(requestEntity.Message);
    }

    [Theory]
    [InlineData(StatusCodes.Status200OK)]
    [InlineData(StatusCodes.Status404NotFound)]
    [InlineData(StatusCodes.Status422UnprocessableEntity)]
    public void Interactor_Return_Different_StatusCode(int status)
    {
        //Arrange
        var requestId = Guid.NewGuid();
        var requestEntity = GetRequestEntity(status, requestId);

        _validator.ValidateAsync(Arg.Any<GetRequestInput>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(new ValidationResult()));
        _memoryCache.GetAsync<RequestEntity>(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                     .Returns(Task.FromResult(requestEntity));

        var sut = GenerateInteractor();

        //Act
        var output = sut.GetAsync(new GetRequestInput(requestId), CancellationToken.None).Result;

        //Assert
        output.StatusCode.Should().Be(status);
    }

    private RequestEntity GetRequestEntity(int StatusCode,Guid requestId)
    {
        var requestEntity = new RequestEntity
        {
            RequestId = requestId,
            Status = RequestStatus.NotStarted
        };

        if (StatusCode.Equals(StatusCodes.Status404NotFound))
        {
            requestEntity = null;
        }

        if (StatusCode.Equals(StatusCodes.Status422UnprocessableEntity))
        {
            requestEntity.Status = RequestStatus.InvalidInformation;
        }

        return requestEntity;
    }


    private GetRequestInteractor GenerateInteractor() =>
        new(_logger, _memoryCache, _validator);
}
