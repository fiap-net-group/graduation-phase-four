using System.Diagnostics.CodeAnalysis;
using Graduation.Notification.Domain.Gateways.Event;

namespace Graduation.Notification.Application.Request.UpdateRequestStatus.Boundaries
{
    [ExcludeFromCodeCoverage]
    public class UpdateRequestStatusEvent : BaseEvent<UpdateRequestStatusInput>
    {
        public override string OperationName => nameof(UpdateRequestStatusEvent);
    }
}
