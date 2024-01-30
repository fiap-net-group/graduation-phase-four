using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Graduation.Notification.Application.Email.Send;
using Graduation.Notification.Application.Email.Send.Boundaries;
using Graduation.Notification.Application.Request.CreateRequest;
using Graduation.Notification.Domain.Gateways.Email;
using Graduation.Notification.Domain.Gateways.Logger;
using Graduation.Notification.Domain.ValueObjects;
using Graduation.Notification.UnitTests.Fixtures;
using NSubstitute;

namespace Graduation.Notification.UnitTests.Application.Email
{
    public class SendEmailTests
    {
        private readonly ILoggerManager _logger;
        private readonly IEmailManager _emailManager;
        private readonly IValidator<SendEmailInput> _validator;

        public SendEmailTests()
        {
            _logger = Substitute.For<ILoggerManager>();
            _emailManager = Substitute.For<IEmailManager>();
            _validator = Substitute.For<IValidator<SendEmailInput>>();
            MapperFixture.AddMapper();
        }

        [Fact]
        public void Interactor_ValidInput_ShouldSendEmail()
        {
            //Arrange
            var input = new SendEmailInput("email@email.com", "Example subject", "example body");

            _validator.ValidateAsync(Arg.Any<SendEmailInput>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(new ValidationResult()));
            _emailManager.SendAsync(Arg.Any<EmailData>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

            var sut = GenerateInteractor();

            //Act
            var output = sut.SendAsync(input, CancellationToken.None).Result;

            //Assert
            output.Value.Should().NotBeNull();
            output.Value.Status.Should().Be(RequestStatus.Completed);
            output.Value.Message.Should().Be(ResponseMessage.Default);
        }

        [Theory]
        [InlineData("","Example subject", "Example body", ResponseMessage.InvalidEmail)]
        [InlineData("email@email.com", "", "Example body", ResponseMessage.InvalidEmailSubject)]
        [InlineData("email@email.com", "Example subject", "", ResponseMessage.InvalidEmailBody)]
        public void Interactor_InvalidInput_ShouldReturnInvalid(string to, string subject, string body, ResponseMessage expectedMessage)
        {
            //Arrange
            var input = new SendEmailInput(to, subject, body);

            var sut = GenerateInteractor(validator: new SendEmailValidator());

            //Act
            var output = sut.SendAsync(input, CancellationToken.None).Result;

            //Assert
            output.Value.Should().NotBeNull();
            output.Value.Status.Should().Be(RequestStatus.InvalidInformation);
            output.Value.Message.Should().Be(expectedMessage);
        }

        [Theory]
        [InlineData("email@email.com", true)]
        [InlineData("", false, ResponseMessage.InvalidEmail)]
        [InlineData("   ", false, ResponseMessage.InvalidEmail)]
        [InlineData("email.com", false, ResponseMessage.InvalidEmail)]
        public void Validator_To_ShouldRespectValidations(string to, bool expectedValid, ResponseMessage? expectedMessage = null)
        {
            //Arrange
            var input = new SendEmailInput(to, "Example subject", "example body");
            var validator = new SendEmailValidator();

            //Act
            var validationResult = validator.Validate(input);

            //Assert
            validationResult.IsValid.Should().Be(expectedValid);
            if (expectedMessage != null)
                validationResult.Errors[0].ErrorMessage.Should().Be(expectedMessage.ToString());
        }

        [Theory]
        [InlineData("Example subject", true)]
        [InlineData("", false, ResponseMessage.InvalidEmailSubject)]
        [InlineData("   ", false, ResponseMessage.InvalidEmailSubject)]
        public void Validator_Subject_ShouldRespectValidations(string subject, bool expectedValid, ResponseMessage? expectedMessage = null)
        {
            //Arrange
            var input = new SendEmailInput("email@email.com", subject, "example body");
            var validator = new SendEmailValidator();

            //Act
            var validationResult = validator.Validate(input);

            //Assert
            validationResult.IsValid.Should().Be(expectedValid);
            if (expectedMessage != null)
                validationResult.Errors[0].ErrorMessage.Should().Be(expectedMessage.ToString());
        }

        [Theory]
        [InlineData("example body", true)]
        [InlineData("", false, ResponseMessage.InvalidEmailBody)]
        [InlineData("   ", false, ResponseMessage.InvalidEmailBody)]
        public void Validator_Body_ShouldRespectValidations(string body, bool expectedValid, ResponseMessage? expectedMessage = null)
        {
            //Arrange
            var input = new SendEmailInput("email@email.com", "Example subject", body);
            var validator = new SendEmailValidator();

            //Act
            var validationResult = validator.Validate(input);

            //Assert
            validationResult.IsValid.Should().Be(expectedValid);
            if (expectedMessage != null)
                validationResult.Errors[0].ErrorMessage.Should().Be(expectedMessage.ToString());
        }

        private SendEmailInteractor GenerateInteractor(ILoggerManager logger = null, IEmailManager emailManager = null, IValidator<SendEmailInput> validator = null)
        {
            return new(logger ?? _logger, emailManager ?? _emailManager, validator ?? _validator);
        }
            
    }
}
