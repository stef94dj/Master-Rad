using MasterRad.Models;
using MasterRad.Models.Configuration;
using MasterRad.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad
{
    public class BaseUserMapController : BaseController
    {
        private readonly IUserRepository _userRepo;
        private readonly SqlServerAdminConnection _adminConnectionConf;
        public BaseUserMapController
        (
            IUserRepository userRepo,
            IOptions<SqlServerAdminConnection> adminConnectionConf
        ) : base()
        {
            _userRepo = userRepo;
            _adminConnectionConf = adminConnectionConf.Value;
        }

        private string _sqlUsername;
        public string SqlUsername
        {
            get
            {
                if (string.IsNullOrEmpty(_sqlUsername))
                    SetSqlCredentials();
                return _sqlUsername;
            }
        }

        private string _sqlPassword;
        public string SqlPassword
        {
            get
            {
                if (string.IsNullOrEmpty(_sqlPassword))
                    SetSqlCredentials();
                return _sqlPassword;
            }
        }

        [NonAction]
        private void SetSqlCredentials()
        {
            if (User.IsInRole(UserRole.Professor))
            {
                _sqlUsername = _adminConnectionConf.Login;
                _sqlPassword = _adminConnectionConf.Password;
            }
            else if (User.IsInRole(UserRole.Student))
            {
                var userMapEntity = _userRepo.Get(UserId);
                _sqlUsername = userMapEntity.SqlUsername;
                _sqlPassword = userMapEntity.SqlPassword;
            }
            else
                throw new UnauthorizedAccessException("Unknown user role.");
        }

        [NonAction]
        protected ConnectionParams GetSqlConnection(string dbName)
            => new ConnectionParams(dbName, SqlUsername, SqlPassword);
    }
}
