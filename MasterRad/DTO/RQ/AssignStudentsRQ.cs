using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RQ
{
    public class AssignStudentsRQ
    {
        public TestType TestType { get; set; }
        public int TestId { get; set; }
        public List<int> StudentIds { get; set; }
    }
}
