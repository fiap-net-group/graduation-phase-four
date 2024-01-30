using Graduation.Notification.Application.Request.CreateRequest;
using Graduation.Notification.Application.Request.CreateRequest.Boundaries;
using Graduation.Notification.Application.Request.GetRequest;
using Graduation.Notification.Application.Request.GetRequest.Boundaries;
using Graduation.Notification.Domain.Gateways.Event;
using Graduation.Notification.Domain.Responses;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Graduation.Notification.API.Features.Core;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[ExcludeFromCodeCoverage]
public class ApiController : ControllerBase
{
    private readonly int _cancelRequisitionAfterInSeconds;
    private readonly ICreateRequestUseCase _createRequest;
    private readonly IGetRequestUseCase _getRequest;

    public ApiController(IConfiguration configuration, ICreateRequestUseCase createRequest, IGetRequestUseCase getRequest)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _cancelRequisitionAfterInSeconds = configuration.GetValue<int>("CancelRequisitionAfterInSeconds", 30);
        _createRequest = createRequest;
        _getRequest = getRequest;
    }

    protected CancellationToken AsCombinedCancellationToken(CancellationToken requestCancellationToken)
    {
        using var combinedCancellationTokens = CancellationTokenSource.CreateLinkedTokenSource(requestCancellationToken, HttpContext.RequestAborted);

        combinedCancellationTokens.CancelAfter(_cancelRequisitionAfterInSeconds);

        return combinedCancellationTokens.Token;
    }

    protected async Task<IActionResult> CreateRequest<TEvent, TInput>(TInput input, CancellationToken cancellationToken) where TEvent : BaseEvent<TInput>
    {
        var output = await _createRequest.CreateAsync<TEvent, TInput>(input.Adapt<CreateRequestInput>(), AsCombinedCancellationToken(cancellationToken));

        var response = new BaseResponseWithValue<CreateRequestOutput>().AsSuccess(output);

        return Accepted(response);
    }
    protected async Task<IActionResult> GetRequest<T>(T input, CancellationToken cancellationToken)
    {
        var output = await _getRequest.GetAsync(input.Adapt<GetRequestInput>(), AsCombinedCancellationToken(cancellationToken));

        var response = new BaseResponseWithValue<GetRequestOutput>().AsSuccess(output); 

        return StatusCode(response.Value.StatusCode, response);
    }

}
