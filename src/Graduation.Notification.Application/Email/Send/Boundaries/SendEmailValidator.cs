﻿using FluentValidation;
using Graduation.Notification.Domain.ValueObjects;
using System.Net.Mail;

namespace Graduation.Notification.Application.Email.Send.Boundaries
{
    public sealed class SendEmailValidator : AbstractValidator<SendEmailInput>
    {
        public SendEmailValidator()
        {
            RuleFor(input => input.To)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(ResponseMessage.InvalidEmail.ToString())
                .Custom((to, context) =>
                {
                    if (!MailAddress.TryCreate(to, out _))
                        context.AddFailure(ResponseMessage.InvalidEmail.ToString());
                });

            RuleFor(input => input.Body)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(ResponseMessage.InvalidEmailBody.ToString());

            RuleFor(input => input.Subject)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(ResponseMessage.InvalidEmailSubject.ToString());
        }
    }
}
