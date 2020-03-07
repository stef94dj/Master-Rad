using System.Collections.Generic;

namespace MasterRad.DTOs
{
    public class StartTestEvalulationSynthesis
    {
        public int TestId { get; set; }
        public List<StudentEvaluationRequestSynthesis> EvaluationRequests { get; set; }
    }

    public class StudentEvaluationRequestSynthesis
    {
        public int StudentId { get; set; }
        public bool UseSecretData { get; set; }
    }
}
