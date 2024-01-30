using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using Graduation.Notification.DependencyInjection.Swagger;
using Graduation.Notification.Application;
using Graduation.Notification.Infrastructure;
using MassTransit;
using Graduation.Notification.Domain.Gateways.Event;
using Graduation.Notification.Application.Email.Send.Boundaries;
using Graduation.Notification.Application.Email.Send;

namespace Graduation.Notification.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddApiConfiguration()
                           .AddSwaggerConfiguration()
                           .AddApiApplicationConfiguration()
                           .AddLoggingManager()
                           .AddApiEventManager(configuration)
                           .AddMemoryCacheManager(configuration);
        }

        private static IServiceCollection AddApiConfiguration(this IServiceCollection services)
        {
            services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.Configure<JsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddControllers();

            services.AddEndpointsApiExplorer();

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            return services;
        }

        public static IServiceCollection AddSendEmailWorkerDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddWorkerDependencyInjection<SendEmailService>(configuration, typeof(SendEmailService), nameof(SendEmailEvent))
                           .AddEmailManager(configuration);
        }

        private static IServiceCollection AddWorkerDependencyInjection<TWorker>(this IServiceCollection services, IConfiguration configuration, Type workerType, string queueName)
            where TWorker : class, IConsumer
        {
            return services.AddWorkerApplicationConfiguration()
                           .AddLoggingManager()
                           .AddEventConsumerManager<TWorker>(configuration, workerType, queueName);
        }
    }
}