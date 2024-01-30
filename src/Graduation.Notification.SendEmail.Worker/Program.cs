
using Graduation.Notification.Application.Email.Send;
using Microsoft.Extensions.Configuration;
using Graduation.Notification.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Graduation.Notification.Infrastructure.Email;
using Graduation.Notification.Domain.Gateways.Email;
using Graduation.Notification.Application.Email.Send.Boundaries;

namespace Graduation.Notification.SendEmail.Worker
{
    [ExcludeFromCodeCoverage]
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                   .SetBasePath(builder.Environment.ContentRootPath)
                   .AddJsonFile("appsettings.json", true, true)
                   .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
                   .AddEnvironmentVariables();

            var configuration = builder.Configuration;
            
            builder.Services.AddSendEmailWorkerDependencyInjection(configuration);

            var app = builder.Build();
            
            app.Run();
        }
    }
}
