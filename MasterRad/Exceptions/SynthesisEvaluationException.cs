using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Exceptions
{
    public class SynthesisEvaluationException: Exception
    {
        public SynthesisEvaluationException(bool isSecret, int testId, int studentId, string message): base($"TestId:'{testId}', StudentId:'{studentId}', IsSecret: '{isSecret}'. Synthesis Test Evaluation Error: '{message}'")
        {
           
        }
    }
}
