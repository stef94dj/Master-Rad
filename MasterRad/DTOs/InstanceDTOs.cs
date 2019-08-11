using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTOs
{
    public class IdentifyDTO
    {
        public int Id { get; set; }
    }

    public abstract class UpdateDTO : IdentifyDTO
    {
        public byte[] TimeStamp { get; set; }
    }

    public class DeleteDTO : IdentifyDTO
    {
        public byte[] TimeStamp { get; set; }
    }
}
