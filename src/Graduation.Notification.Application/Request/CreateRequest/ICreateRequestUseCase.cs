using Graduation.Notification.Application.Request.CreateRequest.Boundaries;
using Graduation.Notification.Domain.Gateways.Event;

namespace Graduation.Notification.Application.Request.CreateRequest;

public interface ICreateRequestUseCase
{
    Task<CreateRequestOutput> CreateAsync<TEvent, TEventBody>(CreateRequestInput input, CancellationToken cancellationToken) where TEvent : BaseEvent<TEventBody>;
}
