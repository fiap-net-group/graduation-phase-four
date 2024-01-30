using System.Diagnostics.CodeAnalysis;
using Graduation.Notification.Domain.Entities;

namespace Graduation.Notification.Application.Request.UpdateRequestStatus.Boundaries
{
    [ExcludeFromCodeCoverage]
    public sealed record UpdateRequestStatusInput(RequestEntity Value);
}
