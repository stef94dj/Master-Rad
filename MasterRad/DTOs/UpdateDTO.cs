using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTOs
{
    public abstract class UpdateDTO
    {
        public int Id { get; set; }
        public byte[] TimeStamp { get; set; }
    }
}
