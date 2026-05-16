using Server.Shared.Models;
using System.Diagnostics;
using System.Data.Common;
using Server.Shared.Exceptions;

namespace Server.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainValidationException dex)
            {
                _logger.LogWarning(dex, "Validation error. StatusCode: {StatusCode}, Message: {Message}", dex.StatusCode, string.Join("; ", dex.Errors));
                context.Response.StatusCode = dex.StatusCode;
                context.Response.ContentType = "application/json";

                var errorDetails = new ApiErrorDetails
                {
                    Status = dex.StatusCode,
                    Title = "One or more validation errors occurred.",
                    Detail = string.Join("; ", dex.Errors),
                    TraceId = Activity.Current?.Id ?? context.TraceIdentifier
                };

                await context.Response.WriteAsJsonAsync(errorDetails);
            }
            catch (BusinessRuleValidationException bex)
            {
                _logger.LogError(bex, "Business rule error. StatusCode: {StatusCode}, Message: {Message}", bex.StatusCode, bex.Error);
                context.Response.StatusCode = bex.StatusCode;
                context.Response.ContentType = "application/json";

                var errorDetails = new ApiErrorDetails
                {
                    Status = bex.StatusCode,
                    Title = "An error occurred while processing your request.",
                    Detail = bex.Error,
                    TraceId = Activity.Current?.Id ?? context.TraceIdentifier
                };

                await context.Response.WriteAsJsonAsync(errorDetails);
            }
            catch (AuthorizationException aex)
            {
                _logger.LogError(aex, "Authorization error. StatusCode: {StatusCode}, Message: {Message}", aex.StatusCode, aex.Error);
                context.Response.StatusCode = aex.StatusCode;
                context.Response.ContentType = "application/json";

                var errorDetails = new ApiErrorDetails
                {
                    Status = aex.StatusCode,
                    Title = "Authorization error occurred.",
                    Detail = aex.Error,
                    TraceId = Activity.Current?.Id ?? context.TraceIdentifier
                };

                await context.Response.WriteAsJsonAsync(errorDetails);
            }
            catch (NotFoundException nex)
            {
                _logger.LogError(nex, "Not found error. StatusCode: {StatusCode}, Message: {Message}", nex.StatusCode, nex.Error);
                context.Response.StatusCode = nex.StatusCode;
                context.Response.ContentType = "application/json";

                var errorDetails = new ApiErrorDetails
                {
                    Status = nex.StatusCode,
                    Title = "Data not found.",
                    Detail = nex.Error,
                    TraceId = Activity.Current?.Id ?? context.TraceIdentifier
                };

                await context.Response.WriteAsJsonAsync(errorDetails);
            }
            catch (BadRequestException nex)
            {
                _logger.LogError(nex, "Bad request. StatusCode: {StatusCode}, Message: {Message}", nex.StatusCode, nex.Error);
                context.Response.StatusCode = nex.StatusCode;
                context.Response.ContentType = "application/json";

                var errorDetails = new ApiErrorDetails
                {
                    Status = nex.StatusCode,
                    Title = "Data not found.",
                    Detail = nex.Error,
                    TraceId = Activity.Current?.Id ?? context.TraceIdentifier
                };

                await context.Response.WriteAsJsonAsync(errorDetails);
            }
            catch (DbException dbex)
            {
                _logger.LogError(dbex, "Database error. StatusCode: {StatusCode}, Message: {Message}", 502, dbex.Message);
                context.Response.StatusCode = 502;
                context.Response.ContentType = "application/json";

                var errorDetails = new ApiErrorDetails
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                    Status = 502,
                    Title = "A database error occurred.",
                    Detail = dbex.Message,
                    TraceId = Activity.Current?.Id ?? context.TraceIdentifier
                };

                await context.Response.WriteAsJsonAsync(errorDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception. StatusCode: {StatusCode}, Message: {Message}", 500, ex.GetBaseException().Message);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var errorDetails = new ApiErrorDetails
                {
                    Status = 500,
                    Title = "internal server error",
                    Detail = ex.GetBaseException().Message,
                    TraceId = Activity.Current?.Id ?? context.TraceIdentifier
                };

                await context.Response.WriteAsJsonAsync(errorDetails);
            }
        }
    }
}