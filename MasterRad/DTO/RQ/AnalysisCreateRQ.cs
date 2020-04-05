using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RQ
{
    public class AnalysisCreateRQ
    {
        public string Name { get; set; }
        public int StudentId { get; set; }
        public int SynthesisTestId { get; set; }
    }
}
