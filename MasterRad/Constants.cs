using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad
{
    public static class Constants
    {
        public const string ScopeUserRead = "User.Read";
        public const string ScopeUserReadBasicAll = "User.ReadBasic.All";
        public const string BearerAuthorizationScheme = "Bearer";
        public const string MicrosoftSQLConnectionStringTemplate = "Data Source={0};Initial Catalog={1};User ID={2};Password={3}";
    }
}
