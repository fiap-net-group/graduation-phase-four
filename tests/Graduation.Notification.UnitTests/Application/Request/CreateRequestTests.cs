using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Graduation.Notification.Application.Email.Send.Boundaries;
using Graduation.Notification.Application.Request.CreateRequest;
using Graduation.Notification.Application.Request.CreateRequest.Boundaries;
using Graduation.Notification.Domain.Exceptions;
using Graduation.Notification.Domain.Gateways.Event;
using Graduation.Notification.Domain.Gateways.Logger;
using Graduation.Notification.Domain.Gateways.MemoryCache;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Graduation.Notification.UnitTests.Application.Request;

public sealed class CreateRequestTests
{
    private readonly ILoggerManager _logger;
    private readonly IEventSenderManager _eventManager;
    private readonly IValidator<CreateRequestInput> _validator;
    private readonly IMemoryCacheManager _memoryCache;

    public CreateRequestTests()
    {
        _logger = Substitute.For<ILoggerManager>();
        _eventManager = Substitute.For<IEventSenderManager>();
        _validator = Substitute.For<IValidator<CreateRequestInput>>();
        _memoryCache = Substitute.For<IMemoryCacheManager>();
    }

    [Fact]
    public void Interactor_ValidInput_ShouldSendEvent()
    {
        //Arrange
        var requestId = Guid.NewGuid();
        var inputEvent = new SendEmailEvent();
        var input = new CreateRequestInput(inputEvent.OperationName, inputEvent);

        _validator.ValidateAsync(Arg.Any<CreateRequestInput>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(new ValidationResult()));
        _eventManager.SendAsync<SendEmailEvent, SendEmailInput>(Arg.Any<SendEmailEvent>(), Arg.Any<CancellationToken>())
                     .Returns(Task.FromResult(requestId));

        var sut = GenerateInteractor();

        //Act
        var output = sut.CreateAsync<SendEmailEvent, SendEmailInput>(input, CancellationToken.None).Result;

        //Assert
        output.RequestId.Should().Be(requestId);
    }

    [Fact]
    public void Interactor_InvalidInput_ShouldThrow()
    {
        //Arrange
        var inputEvent = new SendEmailEvent();
        var input = new CreateRequestInput(inputEvent.OperationName, inputEvent);

        _validator.ValidateAsync(Arg.Any<CreateRequestInput>(), Arg.Any<CancellationToken>()).ThrowsAsync<BusinessException>();

        var sut = GenerateInteractor();

        //Act
        var act = async () => await sut.CreateAsync<SendEmailEvent, SendEmailInput>(input, CancellationToken.None);

        //Assert
        act.Should().ThrowExactlyAsync<BusinessException>();
    }

    [Theory]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("  ", false)]
    [InlineData("ExampleOperationName", true)]
    public void Validator_OperationName_ShouldRespectValidations(string operationName, bool expectedValid)
    {
        //Arrange
        var input = new CreateRequestInput(operationName, new { Example = "Test" });
        var sut = new CreateRequestValidator();

        //Act
        var response = sut.Validate(input);

        //Assert
        Assert.Equal(expectedValid, response.IsValid);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void Validator_Value_ShouldRespectValidations(bool nullValue, bool expectedValid)
    {
        //Arrange
        var input = new CreateRequestInput("ExampleOperationName", nullValue ? null : new { Example = "Test" });
        var sut = new CreateRequestValidator();

        //Act
        var response = sut.Validate(input);

        //Assert
        Assert.Equal(expectedValid, response.IsValid);
    }

    private CreateRequestInteractor GenerateInteractor() =>
        new(_logger, _eventManager, _validator, _memoryCache);
}