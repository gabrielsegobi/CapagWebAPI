namespace Application.Exceptions.Base
{
    public abstract class UnauthorizedException : AppException
    {
        protected UnauthorizedException(string message)
            : base(message, 401, "UNAUTHORIZED")
        {
        }
    }
}
