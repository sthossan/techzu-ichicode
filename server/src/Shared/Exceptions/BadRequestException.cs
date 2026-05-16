namespace Server.Shared.Exceptions
{
    public class BadRequestException : Exception
    {
        public string Error { get; }
        public int StatusCode { get; }

        public BadRequestException(string error, int statusCode = 400) : base(error)
        {
            Error = error;
            StatusCode = statusCode;
        }
    }
}
