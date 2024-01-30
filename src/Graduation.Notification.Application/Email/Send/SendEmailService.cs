﻿using Graduation.Notification.Application.Email.Send.Boundaries;
using Graduation.Notification.Application.Request.UpdateRequestStatus.Boundaries;
using Graduation.Notification.Domain.Extensions;
using Graduation.Notification.Domain.Gateways.Event;
using Graduation.Notification.Domain.Gateways.Logger;
using Mapster;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Graduation.Notification.Application.Email.Send
{
    public sealed class SendEmailService : IConsumer<SendEmailEvent>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILoggerManager _logger;

        public SendEmailService(IServiceProvider serviceProvider, ILoggerManager logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SendEmailEvent> context)
        {
            using var scope = _serviceProvider.CreateScope();

            var eventSender = scope.ServiceProvider.GetRequiredService<IEventSenderManager>();
            var sendEmail = scope.ServiceProvider.GetRequiredService<ISendEmailUseCase>();

            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var cancellationToken = configuration.GenerateCancellationToken();

            _logger.Log("Begin executing Send Email business rule", LoggerManagerSeverity.DEBUG, (LoggingConstants.Event, context.Message));
            var output = await sendEmail.SendAsync(context.Message.Value, cancellationToken);
            output.SetRequestId(context.Message.RequestId);

            _logger.Log("End executing Send Email business rule", LoggerManagerSeverity.DEBUG,
                    (LoggingConstants.Event, context.Message),
                    (LoggingConstants.Output, output));

            _logger.Log("Begin saving Send Email business rule response on memory cache", LoggerManagerSeverity.DEBUG);
            await eventSender.SendAsync<UpdateRequestStatusEvent, UpdateRequestStatusInput>(output.Adapt<UpdateRequestStatusEvent>(), cancellationToken);
            _logger.Log("End saving Send Email business rule response on memory cache", LoggerManagerSeverity.DEBUG);
        }
    }
}
