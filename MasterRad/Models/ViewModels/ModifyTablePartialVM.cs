using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models.ViewModels
{
    public class ModifyTablePartialVM : ModifyDatabasePartialVM
    {
        public string TableName { get; set; }
    }
}
