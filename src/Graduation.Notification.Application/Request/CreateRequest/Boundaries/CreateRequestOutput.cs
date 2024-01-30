using System.Diagnostics.CodeAnalysis;

namespace Graduation.Notification.Application.Request.CreateRequest.Boundaries;

[ExcludeFromCodeCoverage]
public sealed record CreateRequestOutput(Guid RequestId);
