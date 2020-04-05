using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RQ
{
    public class DataCreateRQ : DataBaseRQ
    {
        public List<CellDTO> ValuesNew { get; set; }
    }
}
