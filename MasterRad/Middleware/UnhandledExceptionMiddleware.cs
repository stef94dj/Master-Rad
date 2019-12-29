﻿using MasterRad.Entities;
using MasterRad;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

            logRepository.Log(ex, context.Request, ErrorSeverity.Unhandled);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            context.Request.Body.Seek(0, SeekOrigin.Begin);
            string rqBody = null;
            using (var reader = new StreamReader(context.Request.Body))
            {
                rqBody = reader.ReadToEnd();
            };

            dbContext.DetachAllEntities();

            var logEntity = new UnhandledExceptionLogEntity
            {
                Body = rqBody,
                Headers = JsonConvert.SerializeObject(context.Request.Headers),
                Cookies = JsonConvert.SerializeObject(context.Request.Cookies),
                Path = context.Request.Path.ToString(),
                PathBase = context.Request.PathBase.ToString(),
                Method = context.Request.Method,
                Protocol = context.Request.Protocol,
                QueryString = context.Request.QueryString.ToString(),
                Query = JsonConvert.SerializeObject(context.Request.Query)
            };

            logEntity.DateCreated = DateTime.UtcNow;
            logEntity.CreatedBy = "Current user - NOT IMPLEMENTED";
            try
            {
                logEntity.LogMethod = ExceptionLogMethod.JsonSerialize;
                logEntity.Exception = JsonConvert.SerializeObject(ex);
            }
            catch (Exception serializeException)
            {
                logEntity.LogMethod = ExceptionLogMethod.ToString;
                logEntity.SerializeError = JsonConvert.SerializeObject(serializeException);
                logEntity.Exception = ex.ToString();
            };

            dbContext.UnhandledExceptionLog.Add(logEntity);
            dbContext.SaveChanges();
        }
    }
}
