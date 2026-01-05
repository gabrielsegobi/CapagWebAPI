namespace Application.Exceptions.Base
{
    public abstract class AppException : Exception
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }
        public List<string>? Errors { get; }

        protected AppException(string message, int statusCode, string errorCode, IEnumerable<string>? errors = null)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Errors = errors?.ToList();
        }
    }
}
