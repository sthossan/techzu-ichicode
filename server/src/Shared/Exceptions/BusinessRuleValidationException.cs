namespace Server.Shared.Exceptions
{
    public class BusinessRuleValidationException : Exception
    {
        public string Error { get; }
        public int StatusCode { get; }

        public BusinessRuleValidationException(string error, int statusCode = 409) : base(error)
        {
            Error = error;
            StatusCode = statusCode;
        }
    }
}
