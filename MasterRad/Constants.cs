using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad
{
    public static class Constants
    {
        public const string BearerAuthorizationScheme = "Bearer";
        public const string ScopeUserRead = "User.Read";
        public const string ScopeUserReadBasicAll = "User.ReadBasic.All";
        public const string MicrosoftSQLConnectionStringTemplate = "Data Source={0};Initial Catalog={1};User ID={2};Password={3}";
        public const string RoleProfessor = "Professor";
        public const string RoleStudent = "Student";
        public const string JSDateFormat = "yyyy-MM-dd-HH-mm-ss";

    }
}
