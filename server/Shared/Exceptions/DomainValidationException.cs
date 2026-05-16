namespace Server.Shared.Exceptions
{
    public class DomainValidationException : Exception
    {
        public IEnumerable<string> Errors { get; }
        public int StatusCode { get; }

        public DomainValidationException(IEnumerable<string> errors, int statusCode = 400) : base("Validation failed")
        {
            Errors = errors;
            StatusCode = statusCode;
        }
    }
}
