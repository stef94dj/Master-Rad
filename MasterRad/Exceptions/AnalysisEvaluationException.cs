using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Exceptions
{
    public class AnalysisEvaluationException : Exception
    {
        public AnalysisEvaluationException(AnalysisEvaluationType type, int testId, Guid studentId, string message): base($"TestId:'{testId}', StudentId:'{studentId}', Type: '{type}'. Analysis Test Evaluation Error: '{message}'")
        {
           
        }
    }
}
