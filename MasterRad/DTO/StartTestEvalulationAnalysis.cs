using System.Collections.Generic;

namespace MasterRad.DTO
{
    public class StartTestEvalulationAnalysis
    {
        public int TestId { get; set; }
        public List<int> StudentIds { get; set; }
    }
}
