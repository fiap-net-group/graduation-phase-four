using System.ComponentModel;

namespace Graduation.Notification.Domain.Responses
{
    public enum ResponseMessage
    {
        Default,
        ValidationError,
        UnexpectedError,
        InvalidApiKey,
        InvalidEmail,
        InvalidEmailBody,
        InvalidEmailSubject,
    }
}
