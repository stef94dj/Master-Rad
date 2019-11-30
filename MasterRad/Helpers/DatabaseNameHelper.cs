using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Helpers
{
    public static class DatabaseNameHelper
    {
        public static IEnumerable<KeyValuePair<int, string>> SynthesisTestExam(IEnumerable<int> studentIds, int testId)
        {
            return studentIds.Select(sid => new KeyValuePair<int, string>(sid, $"ST_{testId}_{sid}")); //synthesis test
        }

        public static string SynthesisTestEvaluation(int studentId, int testId, bool isSecret)
        {
            if (isSecret)
                return $"STES_{testId}_{studentId}";
            else
                return $"STEP_{testId}_{studentId}";
        }
    }
}
