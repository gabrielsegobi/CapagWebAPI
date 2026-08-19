using Application.Exceptions.Base;

namespace Application.Exceptions.Base
{
    public class BadRequestException : AppException
    {
        public BadRequestException(string message)
            : base(message, 400, "BAD_REQUEST")
        {
        }
    }
}
