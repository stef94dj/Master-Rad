using MasterRad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Helpers
{
    public static class NameHelper
    {
        public static string TemplateName()
        {

            return $"TMP_{Guid.NewGuid()}";
        }

        public static string TaskName()
        {
            return $"TSK_{Guid.NewGuid()}";
        }

        public static IEnumerable<KeyValuePair<Guid, string>> SynthesisTestExam(IEnumerable<Guid> studentIds, int testId)
        {
            return studentIds.Select(sid => new KeyValuePair<Guid, string>(sid, $"ST_{testId}_{sid}")); //synthesis test
        }

        public static IEnumerable<AnalysisAssignModel> AnalysisTestExam(IEnumerable<Guid> studentIds, int testId)
        {
            return studentIds.Select(sid => new AnalysisAssignModel()
            {
                StudentId = sid,
                Database = $"AT_{testId}_{sid}",
                StudentOutputTable = $"AT_SO_{testId}_{sid}",
                TeacherOutputTable = $"AT_TO_{testId}_{sid}"
            });
        }

        public static string SynthesisTestEvaluation(Guid studentId, int testId, bool isSecret)
        {
            if (isSecret)
                return $"STES_{testId}_{studentId}";
            else
                return $"STEP_{testId}_{studentId}";
        }

        public static string AnalysisTestEvaluation(Guid studentId, int testId, AnalysisEvaluationType type)
        {
            return $"ATE_{testId}_{studentId}";
        }

        public static string GenerateRandomSqlPassowrd()
        {
            return $"Pass-{Guid.NewGuid()}-123!";
        }

        public static string GenerateSqlUserName(Guid microsoftId)
        {
            return $"Pass-{Guid.NewGuid()}-123!";
        }
    }
}
