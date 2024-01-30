using Graduation.Notification.API.Features.Core;
using Graduation.Notification.Application.Email.Send.Boundaries;
using Graduation.Notification.Application.Request.CreateRequest;
using Graduation.Notification.Application.Request.CreateRequest.Boundaries;
using Graduation.Notification.Application.Request.GetRequest;
using Graduation.Notification.Application.Request.GetRequest.Boundaries;
using Graduation.Notification.Domain.Entities;
using Graduation.Notification.Domain.Responses;
using MassTransit.Internals;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Graduation.Notification.API.Features.V1.Controllers;

[ApiVersion("1.0")]
[ExcludeFromCodeCoverage]
public class EmailController : ApiController
{
    private readonly IGetRequestUseCase _getRequest;

    public EmailController(IConfiguration configuration, ICreateRequestUseCase createRequest, IGetRequestUseCase getRequest) : base(configuration, createRequest, getRequest)
    {
        _getRequest = getRequest;
    }

    /// <summary>
    /// Creates a email send process
    /// </summary>
    /// <param name="input">The request body</param>
    /// <param name="cancellationToken"></param>
    /// <response code="202">The created request id</response>
    /// <response code="400">The response with the error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted, StatusCode = StatusCodes.Status202Accepted, Type = typeof(BaseResponseWithValue<CreateRequestOutput>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, StatusCode = StatusCodes.Status400BadRequest, Type = typeof(BaseResponse))]
    public async Task<IActionResult> PostSend(SendEmailInput input, CancellationToken cancellationToken) => 
        await CreateRequest<SendEmailEvent, SendEmailInput>(input, cancellationToken);

    /// <summary>
    /// Gets a request by id
    /// </summary>
    /// <param name="input">The request query</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, StatusCode = StatusCodes.Status200OK, Type = typeof(BaseResponseWithValue<GetRequestOutput>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, StatusCode = StatusCodes.Status404NotFound, Type = typeof(BaseResponseWithValue<GetRequestOutput>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, StatusCode = StatusCodes.Status422UnprocessableEntity, Type = typeof(BaseResponseWithValue<GetRequestOutput>))]
    public async Task<IActionResult> GetSend([FromQuery] GetRequestInput input, CancellationToken cancellationToken) =>
        await GetRequest(input, cancellationToken);
}
