using Graduation.Notification.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Graduation.Notification.API;

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

        Console.WriteLine(builder.Environment.EnvironmentName);

        builder.Services.AddApiDependencyInjection(builder.Configuration);

        var app = builder.Build();

        app.UseApiDependencyInjection();

        app.MapControllers();

        app.Run();
    }
}