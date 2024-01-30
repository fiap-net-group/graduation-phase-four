using FluentValidation;

namespace Graduation.Notification.Application.Request.CreateRequest.Boundaries;

public sealed class CreateRequestValidator : AbstractValidator<CreateRequestInput>
{
    public CreateRequestValidator()
    {
        RuleFor(request => request.OperationName).NotEmpty();

        RuleFor(request => request.Value).NotNull();
    }
}
