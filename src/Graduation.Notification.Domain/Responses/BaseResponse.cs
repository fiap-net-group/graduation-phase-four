namespace Graduation.Notification.Domain.Responses
{
    public class BaseResponse
    {
        private ResponseMessage ResponseMessage { get; set; }
        public bool Success { get; set; }
        public ResponseDetails ResponseDetails { get; set; }

        public BaseResponse AsError(ResponseMessage? message = null, params string[] errors)
        {
            errors ??= Array.Empty<string>();
            Success = false;
            ResponseMessage = message ?? ResponseMessage.UnexpectedError;
            ResponseDetails = new ResponseDetails
            {
                Message = ResponseMessage.ToString(),
                Errors = errors
            };
            return this;
        }

        public BaseResponse AsSuccess()
        {
            Success = true;
            ResponseMessage = ResponseMessage.Default;
            ResponseDetails = new ResponseDetails
            {
                Message = ResponseMessage.ToString(),
                Errors = default
            };
            return this;
        }

        public ResponseMessage GetMessage()
        {
            return ResponseMessage;
        }
    }
}
