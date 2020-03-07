using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models
{
    public class EvaluationResult
    {
        public EvaluationResult() { }
        public EvaluationResult(bool match, bool passIfMatch, string Message = "")
        {
            Pass = !(match ^ passIfMatch);
            this.Message = Message;
        }
        public bool Pass { get; set; }
        public string Message { get; set; }

        public EvaluationProgress PassStatus
        {
            get { return Pass ? EvaluationProgress.Passed : EvaluationProgress.Failed; }
        }
    }
}
