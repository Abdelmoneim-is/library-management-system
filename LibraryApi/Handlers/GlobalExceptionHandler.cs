using LibraryDomain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryApi.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
            {
                httpContext.Response.StatusCode = 499;
                return true;
            }

            var (status, title, detail) = exception switch
            {
                ConcurrencyException or DbUpdateConcurrencyException =>
                    (StatusCodes.Status409Conflict, "Concurrency.Conflict",
                     "This data was modified by another request. Please try again."),

                DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 } } =>
                    (StatusCodes.Status409Conflict, "Database.Duplicate",
                     "This record already exists."),

                _ => (StatusCodes.Status500InternalServerError, "Server.Error",
                     _env.IsDevelopment() ? exception.Message : "An unexpected error occurred.")
            };

            if (status == StatusCodes.Status500InternalServerError)
                _logger.LogError(exception, "Unhandled exception on {Method} {Path}",
                    httpContext.Request.Method, httpContext.Request.Path);
            else
                _logger.LogWarning("Handled {Title} on {Method} {Path}",
                    title, httpContext.Request.Method, httpContext.Request.Path);

            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail
            });

            return true;

        }
    }
};
