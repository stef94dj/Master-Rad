using MasterRad.Entities;
using MasterRad.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

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
