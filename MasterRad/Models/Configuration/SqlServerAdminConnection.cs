using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models.Configuration
{
    public class SqlServerAdminConnection
    {
        public string DbName { get; set; }
        public string OutputTablesDbName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string ReadOnlyLogin { get; set; }
        public string ReadOnlyPassword { get; set; }
        public string ServerName { get; set; }
    }
}
