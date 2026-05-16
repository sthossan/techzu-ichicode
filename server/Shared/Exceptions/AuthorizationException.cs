namespace Server.Shared.Exceptions
{
    public class AuthorizationException : Exception
    {
        public string Error { get; }
        public int StatusCode { get; }

        public AuthorizationException(string error, int statusCode = 403) : base(error)
        {
            Error = error;
            StatusCode = statusCode;
        }
    }
}
