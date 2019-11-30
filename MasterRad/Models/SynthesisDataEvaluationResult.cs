using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models
{
    public class SynthesisEvaluationResult
    {
        public bool Pass { get; set; }
        public string FailReason { get; set; }
    }
}
