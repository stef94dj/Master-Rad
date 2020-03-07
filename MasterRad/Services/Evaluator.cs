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
        EvaluationResult EvaluateQueryOutputs(QueryExecuteRS qr1, QueryExecuteRS qr2, IEnumerable<string> expectedFormat, bool shouldMatch);
    }

    public class Evaluator : IEvaluator
    {
        public EvaluationResult EvaluateQueryOutputs(QueryExecuteRS qr1, QueryExecuteRS qr2, IEnumerable<string> expectedFormat, bool shouldMatch)
        {
            #region Validation
            if (expectedFormat.Count() < 1 || expectedFormat.Where(colName => string.IsNullOrEmpty(colName)).Any())
                return new EvaluationResult() { Message = $"Invalid expected result format" };

            if (qr1.Tables.Count() != 1)
                return new EvaluationResult() { Message = $"Query result 1 returned {qr1.Tables.Count()} tables" };

            if (qr2.Tables.Count() != 1)
                return new EvaluationResult() { Message = $"Query result 2 returned {qr1.Tables.Count()} tables" };
            #endregion

            var qr1Table = qr1.Tables[0];
            var qr2Table = qr2.Tables[0];

            #region Validation
            if (expectedFormat.Count() != expectedFormat.Distinct().Count())
                return new EvaluationResult() { Message = "Expected column names are not distinct" };

            if (qr2Table.Columns.Count() != qr2Table.Columns.Distinct().Count())
                return new EvaluationResult() { Message = "Query result 2 table column names that are not distinct" };

            if (qr1Table.Columns.Count() != qr1Table.Columns.Distinct().Count())
                return new EvaluationResult() { Message = "Query result 1 table column names that are not distinct" };

            if (expectedFormat.Count() != expectedFormat.Distinct().Count())
                return new EvaluationResult() { Message = "Expected column names are not distinct" };

            if (qr2Table.Columns.Count() != expectedFormat.Count())
                return new EvaluationResult() { Message = $"QQuery result 2 table returned {qr2Table.Columns.Count()} columns. Expected {expectedFormat.Count()}" };

            if (qr1Table.Columns.Count() != expectedFormat.Count())
                return new EvaluationResult() { Message = $"Query result 1 table returned {qr1Table.Columns.Count()} columns. Expected {expectedFormat.Count()}" };

            if (qr2Table.Columns.Where(col => string.IsNullOrEmpty(col.Name)).Any())
                return new EvaluationResult() { Message = $"Query result 2 table contains invalid column names." };

            if (qr1Table.Columns.Where(col => string.IsNullOrEmpty(col.Name)).Any())
                return new EvaluationResult() { Message = $"Query result 1 table contains invalid column names." };

            foreach (var colName in expectedFormat)
            {
                if (qr2Table.Columns.Select(tc => tc.Name).Count(x => x.Equals(colName)) != 1)
                    return new EvaluationResult() { Message = $"Query result 2 table does not containt expected column '{colName}'." };

                if (qr1Table.Columns.Select(sc => sc.Name).Count(x => x.Equals(colName)) != 1)
                    return new EvaluationResult() { Message = $"Query result 1 table does not containt expected column '{colName}'." };
            }
            #endregion

            var rowDiff = qr1Table.Rows.Count() - qr2Table.Rows.Count();
            if (rowDiff != 0)
            {
                var moreOrLess = rowDiff > 0 ? "more" : "less";
                return new EvaluationResult(false, shouldMatch, $"Query returned {Math.Abs(rowDiff)} records {moreOrLess} than the solution");
            }

            var qr2MatchedRowIndexes = new List<int>();
            for (int qr1RowIndex = 0; qr1RowIndex < qr1Table.Rows.Count(); qr1RowIndex++)
            {
                var qr1Row = qr1Table.Rows[qr1RowIndex];
                var qr1RowIndexInQr2 = IndexInTable(qr2Table, qr1Row, qr1Table.Columns, expectedFormat, qr2MatchedRowIndexes);
                if (qr1RowIndexInQr2 < 0)
                    return new EvaluationResult(false, shouldMatch, $"Row {qr1RowIndex} not found in solution");
                else
                    qr2MatchedRowIndexes.Add(qr1RowIndexInQr2);
            }

            return new EvaluationResult(true, shouldMatch, "Match");
        }

        private int IndexInTable(Table qr2Table, List<object> qr1Row, List<Column> qr1Columns, IEnumerable<string> expectedFormat, List<int> excludeIndexes)
        {
            for (int qr2RowIndex = 0; qr2RowIndex < qr2Table.Rows.Count(); qr2RowIndex++)
            {
                if (excludeIndexes.Contains(qr2RowIndex))
                    continue;

                var qr2Row = qr2Table.Rows[qr2RowIndex];
                if (RowsAreEqual(qr2Row, qr2Table.Columns, qr1Row, qr1Columns, expectedFormat))
                    return qr2RowIndex;
            }
            return -1;
        }

        private bool RowsAreEqual(List<object> qr2Row, List<Column> qr2Columns, List<object> qr1Row, List<Column> qr1Columns, IEnumerable<string> expectedFormat)
        {

            foreach (var columnName in expectedFormat)
            {
                var qr2Index = qr2Columns.FindIndex(tc => string.Equals(columnName, tc.Name, StringComparison.InvariantCultureIgnoreCase));
                var qr1Index = qr1Columns.FindIndex(sc => string.Equals(columnName, sc.Name, StringComparison.InvariantCultureIgnoreCase));

                if (!string.Equals(qr2Row[qr2Index].ToString(), qr1Row[qr1Index].ToString()))
                    return false;
            }
            return true;
        }
    }
}
