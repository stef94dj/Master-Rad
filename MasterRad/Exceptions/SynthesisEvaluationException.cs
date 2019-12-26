using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Exceptions
{
    public class SynthesisEvaluationException: Exception
    {
        public SynthesisEvaluationException(int testId, int studentId, string message): base($"Synthesis Test Evaluation Error. {message}. TestId:'{testId}', StudentId:'{studentId}'")
        {
           
        }
    }
}
