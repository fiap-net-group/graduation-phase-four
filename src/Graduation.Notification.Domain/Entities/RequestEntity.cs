using Graduation.Notification.Domain.ValueObjects;

namespace Graduation.Notification.Domain.Entities
{
    public sealed class RequestEntity
    {
        public Guid RequestId { get; set; }
        public ResponseMessage Message { get; set; }
        public RequestStatus Status { get; set; }

        public RequestEntity() { }

        public RequestEntity(ResponseMessage message, RequestStatus status)
        {
            Message = message;
            Status = status;
        }
    }
}
