using FluentAssertions;
using Graduation.Notification.Application.Email.Send.Boundaries;
using Graduation.Notification.Domain.Extensions;
using Graduation.Notification.Domain.Gateways.Logger;
using Graduation.Notification.UnitTests.Fixtures;
using NSubstitute;

namespace Graduation.Notification.UnitTests.Domain.Extensions
{
    public class ValidatorExtensionsTests
    {
        private readonly ILoggerManager _logger;

        public ValidatorExtensionsTests()
        {
            _logger = Substitute.For<ILoggerManager>();
        }

        [Fact]
        public void ValidateInputAsync_ValidInput_ShouldReturnValidAndNullOutput()
        {
            //Arrange
            var input = new SendEmailInput("email@example.com", "Example subject", "Example body");
            var validator = new SendEmailValidator();

            //Act
            (var validationResponse, var validationOutput) = validator.ValidateInputAsync<SendEmailInput, SendEmailOutput>(input, _logger, CancellationToken.None).Result;

            //Assert
            validationResponse.Should().BeTrue();
            validationOutput.Should().BeNull();
        }

        [Fact]
        public void ValidateInputAsync_InalidInput_ShouldReturnInvalidAndNotNullOutput()
        {
            //Arrange
            MapperFixture.AddMapper();
            var input = new SendEmailInput("", "", "");
            var validator = new SendEmailValidator();

            //Act
            (var validationResponse, var validationOutput) = validator.ValidateInputAsync<SendEmailInput, SendEmailOutput>(input, _logger, CancellationToken.None).Result;

            //Assert
            validationResponse.Should().BeFalse();
            validationOutput.Should().NotBeNull();
        }
    }
}
