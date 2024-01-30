using FluentValidation;
using Graduation.Notification.Domain.Exceptions;
using Graduation.Notification.Domain.Gateways.Logger;
using Graduation.Notification.Domain.Responses;
using Mapster;

namespace Graduation.Notification.Domain.Extensions
{
    public static class ValidatorExtensions
    {
        public static async Task ThrowIfInvalidAsync<TInput>(this IValidator<TInput> validator, TInput input, ILoggerManager logger, CancellationToken cancellationToken)
        {
            logger.Log("Validating the input", LoggerManagerSeverity.DEBUG, (LoggingConstants.Input, input));

            var result = await validator.ValidateAsync(input, cancellationToken);

            if(result.IsValid)
            {
                logger.Log("Input is valid", LoggerManagerSeverity.DEBUG, (LoggingConstants.Input, input));
                return;
            }

            throw new BusinessException(ResponseMessage.ValidationError.ToString(), result.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        public static async Task<(bool IsValid, TOutput Output)> ValidateInputAsync<TInput, TOutput>(this IValidator<TInput> validator, TInput input, ILoggerManager logger, CancellationToken cancellationToken)
            where TInput : class
            where TOutput : class
        {
            logger.Log("Validating the input", LoggerManagerSeverity.DEBUG, (LoggingConstants.Input, input));

            var validation = await validator.ValidateAsync(input, cancellationToken);

            if (!validation.IsValid)
            {
                logger.Log("Input is not valid", LoggerManagerSeverity.WARNING,
                        (LoggingConstants.Input, input),
                        (LoggingConstants.Validation, validation));

                return (false, validation.Adapt<TOutput>());
            }

            return (true, null);
        }
    }
}
