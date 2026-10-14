using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Admin.Dashboard.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // التعامل مع أخطاء الـ Not Found (404) لو الرابط مش موجود
                if (context.Response.StatusCode == (int)HttpStatusCode.NotFound && !context.Response.HasStarted)
                {
                    context.Request.Path = "/Error/NotFound";
                    await _next(context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during the request execution.");

                // إعادة توجيه المستخدم لصفحة الخطأ 500
                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Request.Path = "/Error/ServerInternalError";
                    await _next(context);
                }
            }
        }
    }
}