using Graduation.Notification.Domain.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace Graduation.Notification.Application.Request.CreateRequest.Boundaries;

[ExcludeFromCodeCoverage]
public sealed record CreateRequestInput(string OperationName, object Value);
