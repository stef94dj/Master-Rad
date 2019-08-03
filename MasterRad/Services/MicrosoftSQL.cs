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
using System.Globalization;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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
        IEnumerable<string> GetDatabaseNames();
        IEnumerable<ColumnInfo> GetColumnsData(string tableName, ConnectionParams connParams);
        Result<bool> InsertRecord(string table, List<Cell> record, ConnectionParams connParams);
        Result<bool> UpdateRecord(string table, Cell cellNew, List<Cell> recordPrevious, ConnectionParams connParams);
        Result<bool> DeleteRecord(string table, List<Cell> record, ConnectionParams connParams);
        Result<bool> CreateDatabaseFromScript(string dbName, string sqlScript);
        bool DatabaseExists(string name);
        bool DeleteDatabaseIfExists(string name);
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
                                for (var colIndex = 0; colIndex < columns.Count(); colIndex++)
                                {
                                    var cell = reader[colIndex];
                                    if (cell is DBNull)
                                    {
                                        tableRow.Add(null);
                                        continue;
                                    }

                                    var value = string.Empty;
                                    var columnName = columns[colIndex].ColumnName; // for debug
                                    var sqlType = reader.GetDataTypeName(colIndex);

                                    switch (sqlType)
                                    {
                                        case "date":
                                            value = ((DateTime)cell).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                                            break;
                                        case "time":
                                            value = ((TimeSpan)cell).ToString(@"hh\:mm\:ss\.fffffff", CultureInfo.InvariantCulture);
                                            break;
                                        case "smalldatetime":
                                            value = ((DateTime)cell).ToString("yyyy-MM-ddThh:mm:ss", CultureInfo.InvariantCulture);
                                            break;
                                        case "datetime":
                                            value = ((DateTime)cell).ToString("yyyy-MM-ddThh:mm:ss.fff", CultureInfo.InvariantCulture);
                                            break;
                                        case "datetime2":
                                            value = ((DateTime)cell).ToString("yyyy-MM-ddThh:mm:ss.fffffff", CultureInfo.InvariantCulture);
                                            break;
                                        case "datetimeoffset":
                                            value = ((DateTimeOffset)cell).ToString("yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture);
                                            break;
                                        case "binary":
                                        case "varbinary":
                                        case "timestamp":
                                        case "rowversion":
                                            value = $"0x{BitConverter.ToString((byte[])cell).Replace("-", "")}";
                                            break;
                                        default:
                                            value = Convert.ToString(cell, CultureInfo.InvariantCulture);
                                            break;
                                    }

                                    tableRow.Add(value);
                                }
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

        public IEnumerable<string> GetDatabaseNames()
        {
            var sqlCommand = "SELECT name FROM master.dbo.sysdatabases";

            var sqlResult = ExecuteSQLAsAdmin(sqlCommand);

            return sqlResult
                    .Tables[0]
                    .Rows
                    .Select(x => x[0].ToString());
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
            var cells = record.Select(x => $"'{x.Value}'");
            var sqlCommand = $"INSERT INTO [{table}]({string.Join(", ", columns)}) VALUES ({string.Join(", ", cells)})";

            var sqlResult = ExecuteSQL(sqlCommand, connParams);
            if (sqlResult.Messages.Any())
                return Result<bool>.Fail(sqlResult.Messages);

            return Result<bool>.Success(true);
        }

        public Result<bool> UpdateRecord(string table, Cell cellNew, List<Cell> recordPrevious, ConnectionParams connParams)
        {
            if (!recordPrevious.Any())
                return Result<bool>.Fail(new List<string>() { "Unable to identify record" });

            var columnValuesWhere = recordPrevious.Select(x => $"[{x.ColumnName}] = '{x.Value}'");
            var whereExpr = string.Join(" and ", columnValuesWhere);

            var setExpr = $"[{cellNew.ColumnName}] = '{cellNew.Value}'";

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

        public Result<bool> CreateDatabaseFromScript(string dbName, string sqlScript)
        {
            var createResult = ExecuteSQLAsAdmin(sqlScript);

            if (createResult.Messages.Any())
            {
                DeleteDatabaseIfExists(dbName);
                return Result<bool>.Fail(createResult.Messages);
            }

            return Result<bool>.Success(true);
        }

        public bool DatabaseExists(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return GetDatabaseNames()
                    .Where(x => x.ToLower().Equals(name.ToLower()))
                    .Any();
        }

        public bool DeleteDatabaseIfExists(string name)
        {
            if (!DatabaseExists(name))
                return true;

            var sqlQuery = $"DROP DATABASE [{name}]";

            var deleteResult = ExecuteSQLAsAdmin(sqlQuery);

            return DatabaseExists(name);
        }
    }
}
