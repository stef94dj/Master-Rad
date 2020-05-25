using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RQ
{
    public class UpdateInstanceQuery: UpdateDTO
    {
        public string SqlQuery { get; set; }
    }
}
