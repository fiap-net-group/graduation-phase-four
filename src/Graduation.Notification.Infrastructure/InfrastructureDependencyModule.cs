using Graduation.Notification.Application.Email.Send;
using Graduation.Notification.Application.Email.Send.Boundaries;
using Graduation.Notification.Application.Request.UpdateRequestStatus;
using Graduation.Notification.Application.Request.UpdateRequestStatus.Boundaries;
using Graduation.Notification.Domain.Extensions;
using Graduation.Notification.Domain.Gateways.Email;
using Graduation.Notification.Domain.Gateways.Event;
using Graduation.Notification.Domain.Gateways.Logger;
using Graduation.Notification.Domain.Gateways.MemoryCache;
using Graduation.Notification.Infrastructure.Email;
using Graduation.Notification.Infrastructure.Email.Boundaries;
using Graduation.Notification.Infrastructure.Event;
using Graduation.Notification.Infrastructure.Logger;
using Graduation.Notification.Infrastructure.MemoryCache;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Graduation.Notification.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class InfrastructureDependencyModule
    {
        public static IServiceCollection AddLoggingManager(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, ConsoleLoggerManager>();

            return services;
        }

        public static IServiceCollection AddApiEventManager(this IServiceCollection services, IConfiguration configuration)
            => services.AddEventManager<UpdateRequestStatusService>(configuration, typeof(UpdateRequestStatusService), nameof(UpdateRequestStatusEvent), false);

        public static IServiceCollection AddEventConsumerManager<TWorker>(this IServiceCollection services, IConfiguration configuration, Type workerType, string queueName)
            where TWorker : class, IConsumer
            => services.AddEventManager<TWorker>(configuration, workerType, queueName, true);


        private static IServiceCollection AddEventManager<TWorker>(this IServiceCollection services, IConfiguration configuration, Type workerType, string queueName, bool isWorker)
            where TWorker : class, IConsumer
        {
            services.AddScoped<IEventSenderManager, MassTransitSenderManager>();

            services.AddMassTransit(busConfiguration =>
            {
                busConfiguration.AddBusConfiguration<TWorker>(configuration, queueName, isWorker);

                busConfiguration.AddConsumer(workerType);
            });

            return services;
        }

        private static IBusRegistrationConfigurator AddBusConfiguration<TWorker>(this IBusRegistrationConfigurator busConfiguration, IConfiguration configuration, string queueName, bool isWorker)
            where TWorker : class, IConsumer
        {
            if (configuration.IsProduction())
            {
                busConfiguration.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(configuration.GetValueOrThrow<string>("Gateways:Event:ServiceBus:ConnectionString"), h => { });

                    if (!isWorker)
                        cfg.AddMessagesTypes();

                    cfg.ReceiveEndpoint(queueName, e =>
                    {
                        e.Consumer<TWorker>(context);
                    });
                });
            }
            else
            {
                busConfiguration.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration.GetValueOrThrow<string>("Gateways:Event:RabbitMq:Server"), "/", h =>
                    {
                        h.Username(configuration.GetValueOrThrow<string>("Gateways:Event:RabbitMq:Username"));
                        h.Password(configuration.GetValueOrThrow<string>("Gateways:Event:RabbitMq:Password"));
                    });

                    cfg.ReceiveEndpoint(queueName, e =>
                    {
                        e.Consumer<TWorker>(context);
                    });
                });
            }

            return busConfiguration;
        }

        private static IServiceBusBusFactoryConfigurator AddMessagesTypes(this IServiceBusBusFactoryConfigurator cfg)
        {
            //Insert all messages types that are sent by the API
            cfg.Message<SendEmailEvent>(m => m.SetEntityName(nameof(SendEmailEvent).ToLower()));

            return cfg;
        }

        public static IServiceCollection AddMemoryCacheManager(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddScoped<IMemoryCacheManager, MicrosoftMemoryManager>();
            services.AddSingleton(new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(configuration.GetValue("Gateways:MemoryCache:SlidingExpirationInMinutes", 2))
            });

            return services;
        }

        public static IServiceCollection AddEmailManager(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEmailManager, SmtpEmailManager>();

            services.AddSingleton(provider => new EmailConfiguration(
                 From: configuration.GetValue("Gateways:Email:From", "graduationphasefour@outlook.com"),
                 DisplayName: configuration.GetValue("Gateways:Email:DisplayName", "Graduation Phase Four"),
                 Password: configuration.GetValue("Gateways:Email:Password", "Fiap@123"),
                 Host: configuration.GetValue("Gateways:Email:Host", "smtp.office365.com"),
                 Port: configuration.GetValue("Gateways:Email:Port", 587)));

            return services;
        }
    }
}