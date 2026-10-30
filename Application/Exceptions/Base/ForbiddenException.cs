namespace Application.Exceptions.Base
{
    public abstract class ForbiddenException : AppException
    {
        protected ForbiddenException(string message)
            : base(message, 403, "FORBIDDEN")
        {
        }
    }
}
