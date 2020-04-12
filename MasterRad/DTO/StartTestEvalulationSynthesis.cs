using System;
using System.Collections.Generic;

namespace MasterRad.DTO
{
    public class StartTestEvalulationSynthesis
    {
        public int TestId { get; set; }
        public List<StudentEvaluationRequestSynthesis> EvaluationRequests { get; set; }
    }

    public class StudentEvaluationRequestSynthesis
    {
        public Guid StudentId { get; set; }
        public bool UseSecretData { get; set; }
    }
}
