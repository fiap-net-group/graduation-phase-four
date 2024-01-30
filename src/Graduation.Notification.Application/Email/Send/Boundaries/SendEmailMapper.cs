using FluentValidation.Results;
using Graduation.Notification.Application.Request.CreateRequest.Boundaries;
using Graduation.Notification.Application.Request.UpdateRequestStatus.Boundaries;
using Graduation.Notification.Domain.Entities;
using Graduation.Notification.Domain.Gateways.Email;
using Graduation.Notification.Domain.ValueObjects;
using Mapster;
using System.Diagnostics.CodeAnalysis;

namespace Graduation.Notification.Application.Email.Send.Boundaries
{
    [ExcludeFromCodeCoverage]
    public static class SendEmailMapper
    {
        public static void Add()
        {
            TypeAdapterConfig<SendEmailInput, CreateRequestInput>
                .NewConfig()
                .Map(destination => destination.OperationName, source => nameof(SendEmailEvent))
                .Map(destination => destination.Value, source => (object)source);

            TypeAdapterConfig<CreateRequestInput, SendEmailEvent>
                .NewConfig()
                .Map(destination => destination.RequestId, source => Guid.Empty)
                .Map(destination => destination.Value, source => (SendEmailInput)source.Value);

            TypeAdapterConfig<ValidationResult, SendEmailOutput>
                .NewConfig()
                .ConstructUsing(source =>  new SendEmailOutput(new RequestEntity
                (
                    Enum.Parse<ResponseMessage>(source.Errors[0].ErrorMessage),
                    RequestStatus.InvalidInformation
                )));

            TypeAdapterConfig<SendEmailInput, EmailData>
                .NewConfig()
                .Map(destination => destination.To, source => source.To)
                .Map(destination => destination.HtmlBody, source => source.Body)
                .Map(destination => destination.Subject, source => source.Subject);

            TypeAdapterConfig<SendEmailOutput, UpdateRequestStatusEvent>
                .NewConfig()
                .Map(destination => destination.RequestId, source => source.RequestId)
                .Map(destination => destination.Value, source => new UpdateRequestStatusInput(source.Value));
        }
    }
}
