using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using MasterRad.Repositories;

namespace MasterRad.Middleware
{
    public class UnhandledExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public UnhandledExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, Context dbContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                HandleException(httpContext, ex, dbContext);
            }
        }

        private static void HandleException(HttpContext context, Exception ex, Context dbContext)
        {
            var logRepository = (ILogRepository)context.RequestServices.GetService(typeof(ILogRepository));
            logRepository.Log(ex, context.Request);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
        }
    }
}
