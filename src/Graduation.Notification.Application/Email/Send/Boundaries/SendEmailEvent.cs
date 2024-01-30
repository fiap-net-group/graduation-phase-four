using Graduation.Notification.Domain.Gateways.Event;

namespace Graduation.Notification.Application.Email.Send.Boundaries
{
    public sealed class SendEmailEvent : BaseEvent<SendEmailInput>
    {
        public override string OperationName => nameof(SendEmailEvent);
    }
}
