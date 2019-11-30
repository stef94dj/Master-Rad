using MasterRad.DTOs;
using MasterRad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Services
{
    public interface IEvaluator
    {
        SynthesisEvaluationResult EvaluateSynthesisPaper(QueryExecuteRS studentResult, QueryExecuteRS teacherResult, IEnumerable<string> expectedFormat);
        SynthesisEvaluationResult EvaluateAnalysisPaper(int id);
    }

    public class Evaluator : IEvaluator
    {
        public SynthesisEvaluationResult EvaluateSynthesisPaper(QueryExecuteRS studentResult, QueryExecuteRS teacherResult, IEnumerable<string> expectedFormat)
        {
            if (studentResult.Tables.Count() != 1)
                return new SynthesisEvaluationResult() { Pass = false, FailReason = $"Query returned {studentResult.Tables.Count()} tables" };


            var rowDiff = studentResult.Tables[0].Rows.Count() - teacherResult.Tables[0].Rows.Count();
            if (rowDiff != 0)
            {
                var moreOrLess = rowDiff > 0 ? "more" : "less";
                return new SynthesisEvaluationResult() { Pass = false, FailReason = $"Query returned {Math.Abs(rowDiff)} records {moreOrLess} than the solution" };
            }

            foreach (var studentRow in studentResult.Tables[0].Rows)
            {
                foreach (var column in expectedFormat)
                {
                    throw new NotImplementedException();
                }
            }
        }

        public SynthesisEvaluationResult EvaluateAnalysisPaper(int id)
        {
            throw new NotImplementedException();
        }
    }
}
