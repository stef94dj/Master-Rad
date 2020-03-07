using System.Collections.Generic;

namespace MasterRad.DTOs
{
    public class StartTestEvalulationAnalysis
    {
        public int TestId { get; set; }
        public List<int> StudentIds { get; set; }
    }
}
