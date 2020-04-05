using System.Collections.Generic;

namespace MasterRad.DTO
{
    public class StartTestEvalulation
    {
        public int TestId { get; set; }
        public List<StudentEvaluationRequest> EvaluationRequests { get; set; }
    }

    public class StudentEvaluationRequest
    {
        public int StudentId { get; set; }
        public bool UseSecretData { get; set; }
    }
}
