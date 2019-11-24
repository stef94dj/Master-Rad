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
        Result<bool> EvaluateSynthesisPaper(QueryExecuteRS studentResult, QueryExecuteRS teacherResult, IEnumerable<string> expectedFormat);
        Result<bool> EvaluateAnalysisPaper(int id);
    }

    public class Evaluator : IEvaluator
    {
        public Result<bool> EvaluateSynthesisPaper(QueryExecuteRS studentResult, QueryExecuteRS teacherResult, IEnumerable<string> expectedFormat)
        {
            //Try using overloading of operator == to compare rows
            throw new NotImplementedException();
        }

        public Result<bool> EvaluateAnalysisPaper(int id)
        {
            throw new NotImplementedException();
        }
    }
}
