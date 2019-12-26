using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTOs
{
    public class StartTestEvalulation
    {
        public int TestId { get; set; }
        public List<int> StudentIds { get; set; }
    }
}
