﻿using MasterRad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Helpers
{
    public static class NameHelper
    {
        public static string TemplateName(string templateName)
        {
            return $"TMP_{templateName}".Replace("\t", "_").Replace(" ", "_");
        }

        public static string TaskName(string taskName)
        {
            return $"TSK_{taskName}".Replace("\t", "_").Replace(" ", "_");
        }

        public static IEnumerable<KeyValuePair<int, string>> SynthesisTestExam(IEnumerable<int> studentIds, int testId)
        {
            return studentIds.Select(sid => new KeyValuePair<int, string>(sid, $"ST_{testId}_{sid}")); //synthesis test
        }

        public static IEnumerable<AnalysisAssignModel> AnalysisTestExam(IEnumerable<int> studentIds, int testId)
        {
            return studentIds.Select(sid => new AnalysisAssignModel()
            {
                StudentId = sid,
                Database = $"AT_{testId}_{sid}",
                StudentOutputTable = $"AT_SO_{testId}_{sid}",
                TeacherOutputTable = $"AT_TO_{testId}_{sid}"
            });
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