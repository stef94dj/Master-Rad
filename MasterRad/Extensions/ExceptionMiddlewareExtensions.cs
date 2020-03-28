using MasterRad.Middleware;
using Microsoft.AspNetCore.Builder;

namespace MasterRad.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void AddCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<UnhandledExceptionMiddleware>();
        }
    }
}
