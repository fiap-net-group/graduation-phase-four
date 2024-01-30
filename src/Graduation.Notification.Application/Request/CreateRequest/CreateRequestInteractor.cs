using FluentValidation;
using Graduation.Notification.Application.Request.CreateRequest.Boundaries;
using Graduation.Notification.Domain.Entities;
using Graduation.Notification.Domain.Extensions;
using Graduation.Notification.Domain.Gateways.Event;
using Graduation.Notification.Domain.Gateways.Logger;
using Graduation.Notification.Domain.Gateways.MemoryCache;
using Graduation.Notification.Domain.Responses;
using Mapster;

namespace Graduation.Notification.Application.Request.CreateRequest;

public sealed class CreateRequestInteractor : ICreateRequestUseCase
{
    private readonly ILoggerManager _logger;
    private readonly IEventSenderManager _eventManager;
    private readonly IValidator<CreateRequestInput> _validator;
    private readonly IMemoryCacheManager _memoryCache;

    public CreateRequestInteractor(
        ILoggerManager logger, 
        IEventSenderManager eventManager, 
        IValidator<CreateRequestInput> validator, IMemoryCacheManager memoryCache)
    {
        _logger = logger;
        _eventManager = eventManager;
        _validator = validator;
        _memoryCache = memoryCache;
    }

    public async Task<CreateRequestOutput> CreateAsync<TEvent, TEventBody>(CreateRequestInput input, CancellationToken cancellationToken) 
        where TEvent : BaseEvent<TEventBody>
    {
        _logger.Log("Starting creating the request", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, input));

        await _validator.ThrowIfInvalidAsync(input, _logger, cancellationToken);

        var requestId = await _eventManager.SendAsync<TEvent, TEventBody>(input.Adapt<TEvent>(), cancellationToken);

        var requestEntity = new RequestEntity
        {
            RequestId = requestId,
            Status = Domain.ValueObjects.RequestStatus.NotStarted,
            Message = Domain.ValueObjects.ResponseMessage.Default
        };

        await _memoryCache.CreateOrUpdate(requestId, requestEntity, cancellationToken);

        _logger.Log("Ending creating the request", LoggerManagerSeverity.INFORMATION, (LoggingConstants.RequestEntity, requestEntity));

        return new CreateRequestOutput(requestId);
    }
}
