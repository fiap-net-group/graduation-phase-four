using System.Diagnostics.CodeAnalysis;

namespace Graduation.Notification.Application.Request.GetRequest.Boundaries;

[ExcludeFromCodeCoverage]
public sealed record GetRequestInput(Guid RequestId);