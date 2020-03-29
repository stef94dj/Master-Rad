using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models
{
    public class ConnectionParams
    {
        public string DbName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string ServerName { get; set; }

        public string ConnectionString { get { return $"server={ServerName};database={DbName};User ID={Login};password={Password};"; } }
    }
}
