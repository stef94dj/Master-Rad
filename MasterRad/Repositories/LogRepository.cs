using MasterRad.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Repositories
{
    public interface ILogRepository
    {
        bool Log(Exception ex);
        bool Log(Exception ex, HttpRequest httpRequest);
    }

    public class LogRepository : ILogRepository
    {
        private readonly Context _context;

        public LogRepository(Context context)
        {
            _context = context;
        }

        public bool Log(Exception ex)
        {
            _context.DetachAllEntities();

            var exceptionLogEntity = MapToExceptionLogEntity(ex);

            _context.Add(exceptionLogEntity);
            return _context.SaveChanges() == 1;
        }

        public bool Log(Exception ex, HttpRequest httpRequest)
        {
            _context.DetachAllEntities();

            var exceptionLogEntity = MapToExceptionLogEntity(ex);
            var requestLogEntity = MapToRequestLogEntity(httpRequest);
            requestLogEntity.Exception = exceptionLogEntity;

            _context.Add(exceptionLogEntity);
            _context.Add(requestLogEntity);
            return _context.SaveChanges() == 2;
        }

        private ExceptionLogEntity MapToExceptionLogEntity(Exception ex) //AutoMapper
        {
            var res = new ExceptionLogEntity()
            {
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED"
            };

            try
            {
                res.LogMethod = ExceptionLogMethod.JsonSerialize;
                res.Exception = JsonConvert.SerializeObject(ex);
            }
            catch (Exception serializeException)
            {
                res.LogMethod = ExceptionLogMethod.ToString;
                res.SerializeError = JsonConvert.SerializeObject(serializeException);
                res.Exception = ex.ToString();
            };

            return res;
        }

        private RequestLogEntity MapToRequestLogEntity(HttpRequest httpRequest)
        {
            var res = new RequestLogEntity()
            {
                CreatedBy = "NOT IMPLEMENTED",
                DateCreated = DateTime.UtcNow,
                Headers = JsonConvert.SerializeObject(httpRequest.Headers),
                Cookies = JsonConvert.SerializeObject(httpRequest.Cookies),
                Path = httpRequest.Path.ToString(),
                PathBase = httpRequest.PathBase.ToString(),
                Method = httpRequest.Method,
                Protocol = httpRequest.Protocol,
                QueryString = httpRequest.QueryString.ToString(),
                Query = JsonConvert.SerializeObject(httpRequest.Query)
            };

            if (httpRequest.Body.CanSeek)
            {
                httpRequest.Body.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(httpRequest.Body))
                {
                    res.Body = reader.ReadToEnd();
                };
            }

            return res;
        }
    }
}
