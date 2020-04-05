using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RQ
{
    public class DataDeleteRQ : DataBaseRQ
    {
        public List<CellDTO> ValuesUnmodified { get; set; }
    }
}
