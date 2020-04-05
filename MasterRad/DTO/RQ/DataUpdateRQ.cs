using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RQ
{
    public class DataUpdateRQ : DataBaseRQ
    {
        public CellDTO ValueNew { get; set; }
        public List<CellDTO> ValuesUnmodified { get; set; }
    }
}
