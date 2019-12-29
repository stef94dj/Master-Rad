using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Repositories
{
    public interface ILogRepository
    {
        void Log(Exception ex, ErrorSeverity severity);
        void Log(Exception ex, HttpRequest httpRequest, ErrorSeverity severity);
    }

    public class LogRepository : ILogRepository
    {
        private readonly Context _context;

        public LogRepository(Context context)
        {
            _context = context;
        }

        public void Log(Exception ex, ErrorSeverity severity)
        {
            throw new NotImplementedException();
        }

        public void Log(Exception ex, HttpRequest httpRequest, ErrorSeverity severity)
        {
            throw new NotImplementedException();
        }
    }
}
