using Graduation.Notification.Application.Request.UpdateRequestStatus.Boundaries;
namespace Graduation.Notification.Application;

public interface IUpdateRequestStatusUseCase
{
    Task UpdateAsync(UpdateRequestStatusInput request, CancellationToken cancellationToken);
}
