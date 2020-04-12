using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RQ
{
    public class RemoveAssignedStudentRQ
    {
        public TestType TestType { get; set; }
        public int TestId { get; set; }
        public Guid StudentId { get; set; }
        public byte[] TimeStamp { get; set; }
    }
}
