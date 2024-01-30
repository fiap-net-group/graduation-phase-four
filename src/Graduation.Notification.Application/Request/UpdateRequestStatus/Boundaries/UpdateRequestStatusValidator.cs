using FluentValidation;
using Graduation.Notification.Application.Request.UpdateRequestStatus.Boundaries;

namespace Graduation.Notification.Application;

public class UpdateRequestStatusValidator : AbstractValidator<UpdateRequestStatusInput>
{
    public UpdateRequestStatusValidator()
    {
        RuleFor(request => request.Value.RequestId).NotEmpty();
    }
}
