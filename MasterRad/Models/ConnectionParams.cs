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

        public ConnectionParams() { }

        public ConnectionParams(string dbName, string login, string password)
        {
            DbName = dbName;
            Login = login;
            Password = password;
        }

        public string EFConnectionString
        {
            get
            {
                return $"server={ServerName};database={DbName};User ID={Login};password={Password};";
            }
        }
    }
}
