using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTOs
{
    public class RemoveAssignedRQ
    {
        public TestType TestType { get; set; }
        public int TestId { get; set; }
        public int StudentId { get; set; }
        public byte[] TimeStamp { get; set; }
    }
}
