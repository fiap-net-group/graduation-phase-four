using FluentValidation;
using Graduation.Notification.Application.Request.UpdateRequestStatus.Boundaries;
using Graduation.Notification.Domain.Gateways.Event;
using Graduation.Notification.Domain.Gateways.Logger;
using Graduation.Notification.Domain.Gateways.MemoryCache;

namespace Graduation.Notification.Application;

public sealed class UpdateRequestStatusInteractor : IUpdateRequestStatusUseCase
{
    private readonly ILoggerManager _logger;
    private readonly IValidator<UpdateRequestStatusInput> _validator;
    private readonly IMemoryCacheManager _memoryCache;

    public UpdateRequestStatusInteractor(ILoggerManager logger, IEventSenderManager eventManager, IValidator<UpdateRequestStatusInput> validator, IMemoryCacheManager memoryCache)
    {
        _logger = logger;
        _validator = validator;
        _memoryCache = memoryCache;
    }

    public async Task UpdateAsync(UpdateRequestStatusInput request, CancellationToken cancellationToken) 
    {
        _logger.Log("Starting updating the request status", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, request));

        var validation = await _validator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.Log("Request is not valid", LoggerManagerSeverity.WARNING,
                    (LoggingConstants.RequestEntity, request),
                    (LoggingConstants.Validation, validation));

            return;
        }

        _logger.Log("Request is valid", LoggerManagerSeverity.DEBUG, (LoggingConstants.RequestEntity, request));

        _logger.Log("Updating the request status on memory cache", LoggerManagerSeverity.DEBUG, (LoggingConstants.RequestEntity, request));
        await _memoryCache.CreateOrUpdate(request.Value.RequestId, request.Value, cancellationToken);
        _logger.Log("Request status updated on memory cache", LoggerManagerSeverity.DEBUG, (LoggingConstants.RequestEntity, request));

        _logger.Log("Sending the request status updated event", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, request));

    }
}
