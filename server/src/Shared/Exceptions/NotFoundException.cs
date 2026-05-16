namespace Server.Shared.Exceptions
{
    public class NotFoundException : Exception
    {
        public string Error { get; }
        public int StatusCode { get; }

        public NotFoundException(string error, int statusCode = 404) : base(error)
        {
            Error = error;
            StatusCode = statusCode;
        }
    }
}
