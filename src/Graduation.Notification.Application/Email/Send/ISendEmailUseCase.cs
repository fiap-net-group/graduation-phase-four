using Graduation.Notification.Application.Email.Send.Boundaries;

namespace Graduation.Notification.Application.Email.Send
{
    public interface ISendEmailUseCase
    {
        Task<SendEmailOutput> SendAsync(SendEmailInput input, CancellationToken cancellationToken);
    }
}
