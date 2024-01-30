using Graduation.Notification.Application.Request.GetRequest.Boundaries;
using Graduation.Notification.Domain.Gateways.Event;

namespace Graduation.Notification.Application.Request.GetRequest
{
    public interface IGetRequestUseCase
    {
        Task<GetRequestOutput> GetAsync(GetRequestInput input, CancellationToken cancellationToken);
    }
}
