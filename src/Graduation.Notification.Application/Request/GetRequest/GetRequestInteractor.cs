using FluentValidation;
using Graduation.Notification.Application.Request.CreateRequest.Boundaries;
using Graduation.Notification.Application.Request.GetRequest.Boundaries;
using Graduation.Notification.Domain.Entities;
using Graduation.Notification.Domain.Extensions;
using Graduation.Notification.Domain.Gateways.Event;
using Graduation.Notification.Domain.Gateways.Logger;
using Graduation.Notification.Domain.Gateways.MemoryCache;

namespace Graduation.Notification.Application.Request.GetRequest
{
    public class GetRequestInteractor : IGetRequestUseCase
    {
        private readonly ILoggerManager _logger;
        private readonly IMemoryCacheManager _memoryCache;
        private readonly IValidator<GetRequestInput> _validator;

        public GetRequestInteractor(ILoggerManager logger, IMemoryCacheManager memoryCache, IValidator<GetRequestInput> validator)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _validator = validator;
        }
        public async Task<GetRequestOutput> GetAsync (GetRequestInput request, CancellationToken cancellationToken)
        {
            _logger.Log("Starting get request status", LoggerManagerSeverity.INFORMATION, ("request", request));

            _logger.Log("Validating the request", LoggerManagerSeverity.DEBUG, ("request", request));
            await _validator.ThrowIfInvalidAsync(request,_logger,cancellationToken);
            _logger.Log("Request is valid", LoggerManagerSeverity.DEBUG, ("request", request));

            var requestEntity = await _memoryCache.GetAsync<RequestEntity>(request.RequestId, cancellationToken);

            _logger.Log("Ending get request status", LoggerManagerSeverity.INFORMATION, ("request", request));

            return new GetRequestOutput(requestEntity);
        }
    }
}
