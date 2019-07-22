using MasterRad.Lifetime;
using MasterRad.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MasterRad.DTOs;
using master_BE;
namespace MasterRad.Services
{
    public interface IMicrosoftSQL : IPerWebRequest
    {
        QueryExecuteRS ExecuteSQLAsAdmin(string sqlQuery, string dbName = "master");
        QueryExecuteRS ExecuteSQL(string sqlQuery, ConnectionParams connParams);
        QueryExecuteRS ReadTable(string dbName, string tableName);
        Result<bool> CreateSQLServerUser(string login);
        Result<bool> AssignSQLServerUserToDb(string userLogin, string dbName);
        Result<bool> DeleteSQLServerUser(string userLogin);
        IEnumerable<string> GetTableNames(ConnectionParams connParams);
        IEnumerable<ColumnInfo> GetColumnsData(string tableName, ConnectionParams connParams);
        Result<bool> InsertRecord(string table, List<Cell> record, ConnectionParams connParams);
        Result<bool> UpdateRecord(string table, List<Cell> recordNew, List<Cell> recordPrevious, ConnectionParams connParams);
        Result<bool> DeleteRecord(string table, List<Cell> record, ConnectionParams connParams);
    }

    public class MicrosoftSQL : IMicrosoftSQL
    {
        private readonly IConfiguration _config;
        private ConnectionParams connParams;

        public MicrosoftSQL(IConfiguration config)
        {
            _config = config;
        }

        private string BuildConnectionString(ConnectionParams connParams)
        {
            var template = Constants.MicrosoftSQLConnectionStringTemplate;
            var serverName = _config.GetSection("DbAdminConnection:ServerName").Value;
            return string.Format(template, serverName, connParams.DbName, connParams.Login, connParams.Password);
        }

        public QueryExecuteRS ExecuteSQLAsAdmin(string sqlQuery, string dbName = "master")
        {
            return ExecuteSQL(sqlQuery, GetAdminConnParams(dbName));
        }

        public QueryExecuteRS ExecuteSQL(string sqlQuery, ConnectionParams connParams)
        {
            var messages = new List<string>();
            var tables = new List<Table>();
            int? rowsAffected = default(int?);

            var connectionString = BuildConnectionString(connParams);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.FireInfoMessageEventOnUserErrors = true;
                connection.InfoMessage += new SqlInfoMessageEventHandler((sender, args) =>
                {
                    messages.Add(args.Message); //args.Errors - zbog FireInfoMessageEventOnUserErrors ne razlikujem errors i messages
                });

                SqlCommand command = new SqlCommand(sqlQuery, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    rowsAffected = reader.RecordsAffected;
                    do
                    {
                        var table = new Table();
                        var columns = reader.GetColumnSchema();
                        if (columns.Any())
                        {
                            table.Columns = columns.Select(c => c.ColumnName).ToList();
                            while (reader.Read())
                            {
                                var tableRow = new List<object>();
                                foreach (var col in columns)
                                    tableRow.Add(reader[col.ColumnName] is DBNull ? null : reader[col.ColumnName].ToString());
                                table.Rows.Add(tableRow);
                            }
                        }
                        tables.Add(table);
                    } while (reader.NextResult());
                }
            }

            return new QueryExecuteRS(messages, tables, rowsAffected);
        }

        public Result<bool> CreateSQLServerUser(string login)
        {
            var sqlCommand = $"CREATE LOGIN [{login}] " +
                              $"WITH PASSWORD='{Guid.NewGuid().ToString()}', " +
                              "DEFAULT_DATABASE=[master], " +
                              "DEFAULT_LANGUAGE=[us_english], " +
                              "CHECK_EXPIRATION=OFF, " +
                              "CHECK_POLICY=ON";

            var connParams = GetAdminConnParams("master");
            var sqlResult = ExecuteSQL(sqlCommand, connParams);

            if (sqlResult.Messages.Any())
                return Result<bool>.Fail(sqlResult.Messages);

            return Result<bool>.Success(true);
        }

        public Result<bool> AssignSQLServerUserToDb(string userLogin, string dbName)
        {
            var connParams = GetAdminConnParams(dbName);
            var sqlCommand = $"CREATE USER [{userLogin}] FOR LOGIN [{userLogin}] WITH DEFAULT_SCHEMA=[dbo] " +
                             $"EXEC sp_addrolemember 'db_datawriter' , '{userLogin}' " +
                             $"EXEC sp_addrolemember 'db_datareader' , '{userLogin}'";

            var sqlResult = ExecuteSQL(sqlCommand, connParams);

            if (sqlResult.Messages.Any())
                return Result<bool>.Fail(sqlResult.Messages);

            return Result<bool>.Success(true);
        }

        public Result<bool> DeleteSQLServerUser(string userLogin)
        {
            var connParams = GetAdminConnParams("master");
            var sqlCommand = $"DROP LOGIN [{userLogin}]";

            var sqlResult = ExecuteSQL(sqlCommand, connParams);

            if (sqlResult.Messages.Any())
                return Result<bool>.Fail(sqlResult.Messages);

            return Result<bool>.Success(true);
        }

        private ConnectionParams GetAdminConnParams(string dbName)
        {
            return connParams = new ConnectionParams()
            {
                DbName = dbName,
                Login = _config.GetSection("DbAdminConnection:Login").Value,
                Password = _config.GetSection("DbAdminConnection:Password").Value
            };
        }

        public IEnumerable<string> GetTableNames(ConnectionParams connParams)
        {
            var sqlCommand = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";

            var sqlResult = ExecuteSQL(sqlCommand, connParams);

            var result = sqlResult.Tables[0].Rows
                .Select(x =>
                    (x[0].ToString().Equals("dbo") ? "" : (x[0].ToString() + ".")) +
                    x[1].ToString())
                .OrderBy(fullName => fullName);

            return result;
        }

        public IEnumerable<ColumnInfo> GetColumnsData(string tableName, ConnectionParams connParams)
        {
            var sqlCommand = "SELECT COLUMN_NAME, COLUMN_DEFAULT, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH " +
                             "FROM INFORMATION_SCHEMA.COLUMNS " +
                             $"WHERE TABLE_NAME = '{tableName}'";

            var sqlResult = ExecuteSQL(sqlCommand, connParams);

            //AutoMapper
            var result = new List<ColumnInfo>();
            foreach (var record in sqlResult.Tables[0].Rows)
            {
                var columnInfo = new ColumnInfo();
                columnInfo.Name = record[0].ToString();
                columnInfo.DefaultValue = record[1]?.ToString() ?? null;
                columnInfo.IsNullable = record[2].ToString().Equals("YES");
                columnInfo.Type = record[3].ToString();
                columnInfo.MaxLength = record[4] == null ? default(int?) : int.Parse(record[4].ToString());
                result.Add(columnInfo);
            }

            return result;
        }

        public Result<bool> InsertRecord(string table, List<Cell> record, ConnectionParams connParams)
        {
            var columns = record.Select(x => $"[{x.ColumnName}]");
            var cells = record.Select(x => x.Value);
            var sqlCommand = $"INSERT INTO [{table}]({string.Join(", ", columns)}) VALUES ({string.Join(", ", cells)})";

            var sqlResult = ExecuteSQL(sqlCommand, connParams);
            if (sqlResult.Messages.Any())
                return Result<bool>.Fail(sqlResult.Messages);

            return Result<bool>.Success(true);
        }

        public Result<bool> UpdateRecord(string table, List<Cell> recordNew, List<Cell> recordPrevious, ConnectionParams connParams)
        {
            var columnValuesSet = recordNew.Select(x => $"[{x.ColumnName}] = '{x.Value}'");
            var setExpr = string.Join(", ", columnValuesSet);

            if (!recordPrevious.Any())
                return Result<bool>.Fail(new List<string>() { "Unable to identify record" });

            var columnValuesWhere = recordPrevious.Select(x => $"[{x.ColumnName}] == '{x.Value}'");
            var whereExpr = string.Join(" and ", columnValuesWhere);

            var sqlCommand = $"UPDATE [{table}] SET {setExpr} WHERE {whereExpr}";

            var sqlResult = ExecuteSQL(sqlCommand, connParams);
            if (sqlResult.Messages.Any())
                return Result<bool>.Fail(sqlResult.Messages);

            return Result<bool>.Success(true);
        }

        public Result<bool> DeleteRecord(string table, List<Cell> record, ConnectionParams connParams)
        {
            if (!record.Any())
                return Result<bool>.Fail(new List<string>() { "Unable to identify record" });

            var columnValuesWhere = record.Select(x => $"[{x.ColumnName}] = '{x.Value}'");
            var whereExpr = string.Join(" and ", columnValuesWhere);

            var sqlCommand = $"DELETE FROM [{table}] WHERE {whereExpr}";

            var sqlResult = ExecuteSQL(sqlCommand, connParams);
            if (sqlResult.Messages.Any())
                return Result<bool>.Fail(sqlResult.Messages);

            return Result<bool>.Success(true);
        }

        public QueryExecuteRS ReadTable(string dbName, string tableName)
        {
            var conn = GetAdminConnParams(dbName);
            var sqlCommand = $"SELECT * FROM {tableName}";

            return ExecuteSQL(sqlCommand, connParams);
        }
    }
}
