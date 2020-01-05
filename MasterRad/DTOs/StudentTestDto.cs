using MasterRad.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTOs
{
    public class StudentTestDto
    {
        public TestType TestType { get; set; }
        private SynthesisTestStudentEntity SynthesisTestStudent { get; set; }
        private AnalysisTestStudentEntity AnalysisTestStudent { get; set; }

        public StudentTestDto(SynthesisTestStudentEntity testStudent)
        {
            TestType = TestType.Synthesis;
            SynthesisTestStudent = testStudent;
        }

        public StudentTestDto(AnalysisTestStudentEntity testStudent)
        {
            TestType = TestType.Analysis;
            AnalysisTestStudent = testStudent;
        }

        public object TestStudent
        {
            get
            {
                switch (TestType)
                {
                    case TestType.Synthesis:
                        return SynthesisTestStudent;
                    case TestType.Analysis:
                        return AnalysisTestStudent;
                    default:
                        return null;
                }
            }
        }
    }
}
