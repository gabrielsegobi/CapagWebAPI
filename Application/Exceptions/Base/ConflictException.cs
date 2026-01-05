namespace Application.Exceptions.Base
{
    public abstract class ConflictException : AppException
    {
        protected ConflictException(string message)
            : base(message, 409, "CONFLICT")
        {
        }
    }
}
