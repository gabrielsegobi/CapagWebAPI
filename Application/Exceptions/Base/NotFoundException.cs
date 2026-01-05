namespace Application.Exceptions.Base
{
    public class NotFoundException(string message) : AppException(message, 404, "NOT_FOUND")
    {
    }
}
