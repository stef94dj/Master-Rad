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
            #region Validation
            if (expectedFormat.Count() < 1 || expectedFormat.Where(colName => string.IsNullOrEmpty(colName)).Any())
                return new SynthesisEvaluationResult() { FailReason = $"Invalid expected result format" };

            if (studentResult.Tables.Count() != 1)
                return new SynthesisEvaluationResult() { FailReason = $"Student query returned {studentResult.Tables.Count()} tables" };

            if (teacherResult.Tables.Count() != 1)
                return new SynthesisEvaluationResult() { FailReason = $"Teacher query returned {studentResult.Tables.Count()} tables" };
            #endregion

            var studentTable = studentResult.Tables[0];
            var teacherTable = teacherResult.Tables[0];

            #region Validation
            if (expectedFormat.Count() != expectedFormat.Distinct().Count())
                return new SynthesisEvaluationResult() { FailReason = "Expected column names are not distinct" };

            if (teacherTable.Columns.Count() != teacherTable.Columns.Distinct().Count())
                return new SynthesisEvaluationResult() { FailReason = "Teacher query column names that are not distinct" };

            if (studentTable.Columns.Count() != studentTable.Columns.Distinct().Count())
                return new SynthesisEvaluationResult() { FailReason = "Student query column names that are not distinct" };

            if (expectedFormat.Count() != expectedFormat.Distinct().Count())
                return new SynthesisEvaluationResult() { FailReason = "Expected column names are not distinct" };

            if (teacherTable.Columns.Count() != expectedFormat.Count())
                return new SynthesisEvaluationResult() { FailReason = $"Teacher query returned {teacherTable.Columns.Count()} columns. Expected {expectedFormat.Count()}" };

            if (studentTable.Columns.Count() != expectedFormat.Count())
                return new SynthesisEvaluationResult() { FailReason = $"Student query returned {studentTable.Columns.Count()} columns. Expected {expectedFormat.Count()}" };

            if (teacherTable.Columns.Where(col => string.IsNullOrEmpty(col.Name)).Any())
                return new SynthesisEvaluationResult() { FailReason = $"Teacher query result contains invalid column names." };

            if (studentTable.Columns.Where(col => string.IsNullOrEmpty(col.Name)).Any())
                return new SynthesisEvaluationResult() { FailReason = $"Student query result contains invalid column names." };

            foreach (var colName in expectedFormat)
            {
                if (teacherTable.Columns.Select(tc => tc.Name).Count(tcn => tcn.Equals(colName)) != 1)
                    return new SynthesisEvaluationResult() { FailReason = $"Teacher query result does not containt expected column '{colName}'." };

                if (studentTable.Columns.Select(sc => sc.Name).Count(scn => scn.Equals(colName)) != 1)
                    return new SynthesisEvaluationResult() { FailReason = $"Teacher query result does not containt expected column '{colName}'." };
            }
            #endregion

            var rowDiff = studentTable.Rows.Count() - teacherTable.Rows.Count();
            if (rowDiff != 0)
            {
                var moreOrLess = rowDiff > 0 ? "more" : "less";
                return new SynthesisEvaluationResult() { FailReason = $"Query returned {Math.Abs(rowDiff)} records {moreOrLess} than the solution" };
            }

            var teacherResultMatchedRowIndexes = new List<int>();
            for (int studentRowIndex = 0; studentRowIndex < studentResult.Tables[0].Rows.Count(); studentRowIndex++)
            {
                var studentRow = studentResult.Tables[0].Rows[studentRowIndex];
                var studentRowIndexInSolution = IndexInTable(teacherTable, studentRow, studentTable.Columns, expectedFormat, teacherResultMatchedRowIndexes);
                if (studentRowIndexInSolution < 0)
                    return new SynthesisEvaluationResult() { FailReason = $"Row {studentRowIndex} not found in solution" };
                else
                    teacherResultMatchedRowIndexes.Add(studentRowIndexInSolution);
            }

            return new SynthesisEvaluationResult() { Pass = true };
        }

        private int IndexInTable(Table teacherTable, List<object> studentRow, List<Column> studentColumns, IEnumerable<string> expectedFormat, List<int> excludeIndexes)
        {
            for (int teacherRowIndex = 0; teacherRowIndex < teacherTable.Rows.Count(); teacherRowIndex++)
            {
                if (excludeIndexes.Contains(teacherRowIndex))
                    continue;

                var teacherRow = teacherTable.Rows[teacherRowIndex];
                if (RowsAreEqual(teacherRow, teacherTable.Columns, studentRow, studentColumns, expectedFormat))
                    return teacherRowIndex;
            }
            return -1;
        }

        private bool RowsAreEqual(List<object> teacherRow, List<Column> teacherColumns, List<object> studentRow, List<Column> studentColumns, IEnumerable<string> expectedFormat)
        {

            foreach (var columnName in expectedFormat)
            {
                var teacherColumnIndex = teacherColumns.FindIndex(tc => string.Equals(columnName, tc.Name, StringComparison.InvariantCultureIgnoreCase));
                var studentColumnIndex = studentColumns.FindIndex(sc => string.Equals(columnName, sc.Name, StringComparison.InvariantCultureIgnoreCase));

                if (!string.Equals(teacherRow[teacherColumnIndex].ToString(), studentRow[studentColumnIndex].ToString()))
                    return false;
            }

            return true;
        }

        public SynthesisEvaluationResult EvaluateAnalysisPaper(int id)
        {
            throw new NotImplementedException();
        }
    }
}
