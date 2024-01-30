using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduation.Notification.Application.Request.GetRequest.Boundaries
{
    public class GetRequestValidator : AbstractValidator<GetRequestInput>
    {
        public GetRequestValidator()
        {
            RuleFor(request => request.RequestId).NotEmpty();
        }
    }
}
