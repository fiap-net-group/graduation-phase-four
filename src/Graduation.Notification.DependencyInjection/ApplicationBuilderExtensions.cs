using Graduation.Notification.DependencyInjection.Middlewares;
using Graduation.Notification.DependencyInjection.Swagger;
using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;

namespace Graduation.Notification.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseApiDependencyInjection(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseSwaggerConfiguration();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseMiddleware<ApiKeyMiddleware>();

            return app;
        }

        public static IApplicationBuilder UseWorkerDependencyInjection(this IApplicationBuilder app)
        {
            return app;
        }
    }
}
