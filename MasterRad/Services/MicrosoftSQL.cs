using MasterRad.Lifetime;
using MasterRad.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using MasterRad.DTO;
using System.IO;
using MasterRad.Models.Configuration;
using Microsoft.Extensions.Options;
using MasterRad.DTO.RS;
using MasterRad.Helpers;

namespace MasterRad.Services
{
    public interface IMicrosoftSQL : IPerWebRequest
    {
        #region SQL_Manager_Base
        ConnectionParams GetAdminConnParams(string dbName);
        ConnectionParams GetReadOnlyAdminConnParams(string dbName);
        QueryExecuteRS ExecuteSQLAsAdmin(string sqlQuery, string dbName = "master");
        QueryExecuteRS ExecuteSQLAsReadOnlyAdmin(string sqlQuery, string dbName = "master");
        QueryExecuteRS ExecuteSQL(string sqlQuery, ConnectionParams connParams);
        #endregion

        #region SQL_Metadata_Manager
        IEnumerable<string> GetTableNames(ConnectionParams connParams);
        IEnumerable<TableWithColumns> GetTableNamesWithColumnNames(ConnectionParams connParams);
        IEnumerable<string> GetDatabaseNames();
        IEnumerable<ColumnInfoDTO> GetColumnsData(string schemaName, string tableName, ConnectionParams connParams);
        IEnumerable<ConstraintDTO> GetConstraintData(string schemaName, string tableName, ConnectionParams connParams);
        IEnumerable<string> GetIdentityColumns(string schemaName, string tableName, ConnectionParams connParams);
        #endregion

        #region Record
        int Count(string schemaName, string tableName, List<CellDTO> recordPrevious, ConnectionParams connParams);

        #endregion

        #region Table
        bool CreateTable(CreateTable table);
        IEnumerable<string> CreateTables(IEnumerable<CreateTable> tables);
        bool DeleteTableIfExists(string tableName, string databaseName);
        QueryExecuteRS ReadTable(string schemaName, string tableName, ConnectionParams connParams);
        Result<bool> InsertRecord(string schemaName, string tableName, List<CellDTO> record, ConnectionParams connParams);
        Result<bool> UpdateRecord(string schemaName, string tableName, CellDTO cellNew, List<CellDTO> recordPrevious, ConnectionParams connParams);
        Result<bool> DeleteRecord(string schemaName, string tableName, List<CellDTO> record, ConnectionParams connParams);
        #endregion

        #region Database
        bool CreateDatabase(string dbName, bool contained = false);
        bool CloneDatabase(string originDbName, string destDbName, bool failIfExists);
        IEnumerable<string> CloneDatabases(string originDbName, IEnumerable<string> destDbName, bool failIfExists);
        bool DatabaseExists(string name);
        bool DeleteDatabaseIfExists(string name);
        #endregion

        #region SQL_User_Manager
        IEnumerable<string> GetDatabaseUsers(string dbName);
        bool UserExists(string username, string dbName);
        bool CreateServerLogin(string login, string password);
        bool CreateDbUserFromLogin(string userLogin, string dbName);
        bool CreateDbUserContained(string userName, string password, string dbName);
        bool DeleteDbUser(string userName, string dbName);
        bool DeleteServerLogin(string userLogin);

        bool AssignReadonly(string userName, string dbName);
        bool AssignCRUD(string userName, string dbName);
        bool AssignReadonly(string userName, string dbName, string tableName, string schemaName = null);
        bool AssignCRUD(string userName, string dbName, string tableName, string schemaName = null);
        #endregion 
    }

    public class MicrosoftSQL : IMicrosoftSQL
    {
        private readonly IMsSqlQueryBuilder _msSqlQueryBuilder;
        private ConnectionParams connParams;
        private readonly SqlServerAdminConnection _adminConnectionConf;

        public MicrosoftSQL
        (
            IMsSqlQueryBuilder msSqlQueryBuilder,
            IOptions<SqlServerAdminConnection> adminConnectionConf
        )
        {
            _msSqlQueryBuilder = msSqlQueryBuilder;
            _adminConnectionConf = adminConnectionConf.Value;
        }

        #region SQL_Manager_Base
        public ConnectionParams GetAdminConnParams(string dbName)
        {
            return connParams = new ConnectionParams()
            {
                DbName = dbName,
                Login = _adminConnectionConf.Login,
                Password = _adminConnectionConf.Password
            };
        }
        public ConnectionParams GetReadOnlyAdminConnParams(string dbName)
        {
            return connParams = new ConnectionParams()
            {
                DbName = dbName,
                Login = _adminConnectionConf.ReadOnlyLogin,
                Password = _adminConnectionConf.ReadOnlyPassword
            };
        }
        private string BuildConnectionString(ConnectionParams connParams)
        {
            var template = Constants.MicrosoftSQLConnectionStringTemplate;
            var serverName = _adminConnectionConf.ServerName;
            return string.Format(template, serverName, connParams.DbName, connParams.Login, connParams.Password);
        }
        public QueryExecuteRS ExecuteSQLAsAdmin(string sqlQuery, string dbName = "master")
        {
            return ExecuteSQL(sqlQuery, GetAdminConnParams(dbName));
        }
        public QueryExecuteRS ExecuteSQLAsReadOnlyAdmin(string sqlQuery, string dbName = "master")
        {
            return ExecuteSQL(sqlQuery, GetReadOnlyAdminConnParams(dbName));
        }
        public QueryExecuteRS ExecuteSQL(string sqlQuery, ConnectionParams connParams)
        {
            var messages = new List<string>();
            var tables = new List<TableDTO>();
            int? rowsAffected = default(int?);

            var connectionString = BuildConnectionString(connParams);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.FireInfoMessageEventOnUserErrors = true;
                connection.InfoMessage += new SqlInfoMessageEventHandler((sender, args) =>
                {
                    messages.Add(args.Message);
                });

                SqlCommand command = new SqlCommand(sqlQuery, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    rowsAffected = reader.RecordsAffected;
                    do
                    {
                        var table = new TableDTO();
                        var columns = reader.GetColumnSchema();
                        if (columns.Any())
                        {
                            table.Columns = columns.Select(c => new ColumnDTO(c.ColumnName, c.DataTypeName)).ToList();

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

                                    var value = _msSqlQueryBuilder.ToDisplay(reader.GetDataTypeName(colIndex), cell);
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
        #endregion

        #region SQL_Metadata_Manager
        public IEnumerable<string> GetTableNames(ConnectionParams connParams)
        {
            var sqlCommand = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";

            var sqlResult = ExecuteSQL(sqlCommand, connParams);

            var result = sqlResult.Tables[0].Rows
                .Select(x =>
                    $"{x[0].ToString()}.{x[1].ToString()}")
                    .OrderBy(fullName => fullName);

            return result;
        }
        public IEnumerable<TableWithColumns> GetTableNamesWithColumnNames(ConnectionParams connParams)
        {
            var sqlCommand = "SELECT t.TABLE_SCHEMA, t.TABLE_NAME, c.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLES " +
                             "T INNER JOIN INFORMATION_SCHEMA.COLUMNS C ON t.TABLE_NAME = c.TABLE_NAME and t.TABLE_SCHEMA = c.TABLE_SCHEMA";

            var sqlResult = ExecuteSQL(sqlCommand, connParams);

            var queryResult = sqlResult.Tables[0].Rows
                .Select(x => new TableColumnDataResult()
                {
                    SchemaName = x[0].ToString(),
                    TableName = x[1].ToString(),
                    ColumnName = x[2].ToString()
                });

            var result = queryResult
                         .GroupBy(keyDef => new
                         {
                             keyDef.SchemaName,
                             keyDef.TableName
                         })
                         .Select(group => new TableWithColumns()
                         {
                             TableFullName = $"{group.Key.SchemaName}.{group.Key.TableName}",
                             ColumnNames = group.Select(groupItem => groupItem.ColumnName)
                         });

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
        public IEnumerable<ColumnInfoDTO> GetColumnsData(string schemaName, string tableName, ConnectionParams connParams)
        {
            var sqlCommand = "SELECT COLUMN_NAME, COLUMN_DEFAULT, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH " +
                             "FROM INFORMATION_SCHEMA.COLUMNS " +
                             $"WHERE TABLE_SCHEMA = '{schemaName}' AND TABLE_NAME = '{tableName}'";

            var sqlResult = ExecuteSQL(sqlCommand, connParams);

            //AutoMapper
            var result = new List<ColumnInfoDTO>();
            foreach (var record in sqlResult.Tables[0].Rows)
            {
                var columnInfo = new ColumnInfoDTO();
                columnInfo.Name = record[0].ToString();
                columnInfo.DefaultValue = record[1]?.ToString() ?? null;
                columnInfo.IsNullable = record[2].ToString().Equals("YES");
                columnInfo.Type = record[3].ToString();
                columnInfo.MaxLength = record[4] == null ? default(int?) : int.Parse(record[4].ToString());
                result.Add(columnInfo);
            }

            return result;
        }
        public IEnumerable<ConstraintDTO> GetConstraintData(string schemaName, string tableName, ConnectionParams connParams)
        {
            var sqlCommand = File.ReadAllText(@"SqlScripts\GetTableConstraints.sql");
            sqlCommand = sqlCommand.Replace("#SCHEMATABLENAME", $"{schemaName}.{tableName}");

            var sqlResult = ExecuteSQL(sqlCommand, connParams);

            //AutoMapper
            var result = new List<ConstraintDTO>();
            foreach (var record in sqlResult.Tables[0].Rows)
            {
                var constraintInfo = new ConstraintDTO();
                constraintInfo.Name = record[3].ToString();
                constraintInfo.Type = record[2]?.ToString();
                constraintInfo.Description = record[4].ToString();
                result.Add(constraintInfo);
            }

            return result;
        }
        public IEnumerable<string> GetIdentityColumns(string schemaName, string tableName, ConnectionParams connParams)
        {
            var sqlCommand = "SELECT COLUMN_NAME " +
                             "FROM INFORMATION_SCHEMA.COLUMNS " +
                             $"WHERE COLUMNPROPERTY(object_id('{schemaName}.{tableName}'), COLUMN_NAME, 'IsIdentity') = 1 AND TABLE_SCHEMA = '{schemaName}' AND TABLE_NAME = '{tableName}'";

            var sqlResult = ExecuteSQL(sqlCommand, connParams);
            var res = sqlResult.Tables[0].Rows.Select(r => r[0].ToString());

            return res;
        }
        #endregion

        #region Record
        public Result<bool> InsertRecord(string schemaName, string tableName, List<CellDTO> record, ConnectionParams connParams)
        {
            var columns = record.Select(x => $"[{x.ColumnName}]");
            var cells = record.Select(x => $"'{x.Value}'");
            var sqlCommand = $"INSERT INTO [{schemaName}].[{tableName}]({string.Join(", ", columns)}) VALUES ({string.Join(", ", cells)})";

            var sqlResult = ExecuteSQL(sqlCommand, connParams);
            if (sqlResult.Messages.Any())
                return Result<bool>.Fail(sqlResult.Messages);

            return Result<bool>.Success(true);
        }
        public Result<bool> UpdateRecord(string schemaName, string tableName, CellDTO cellNew, List<CellDTO> recordPrevious, ConnectionParams connParams)
        {
            if (!recordPrevious.Any())
                return Result<bool>.Fail(new List<string>() { "Unable to identify record" });

            var columnValuesWhere = recordPrevious.Select(x =>
                x.Value.ToLower().Equals("null") ? $"[{x.ColumnName}] IS NULL" : $"[{x.ColumnName}] = {_msSqlQueryBuilder.ToQuery(x.ColumnType, x.Value)}"
            );
            var whereExpr = string.Join(" and ", columnValuesWhere);

            var setExpr = $"[{cellNew.ColumnName}] = {_msSqlQueryBuilder.ToQuery(cellNew.ColumnType, cellNew.Value)}";

            var sqlCommand = $"UPDATE [{schemaName}].[{tableName}] SET {setExpr} WHERE {whereExpr}";

            var sqlResult = ExecuteSQL(sqlCommand, connParams);

            if (sqlResult.RowsAffected != 1)
                sqlResult.Messages.Add($"Total of {sqlResult.RowsAffected} rows updated");

            if (sqlResult.Messages.Any())
                return Result<bool>.Fail(sqlResult.Messages);

            return Result<bool>.Success(true);
        }
        public Result<bool> DeleteRecord(string schemaName, string tableName, List<CellDTO> record, ConnectionParams connParams)
        {
            if (!record.Any())
                return Result<bool>.Fail(new List<string>() { "Unable to identify record" });

            var columnValuesWhere = record.Select(x => $"[{x.ColumnName}] = '{x.Value}'");
            var whereExpr = string.Join(" and ", columnValuesWhere);

            var sqlCommand = $"DELETE FROM [{schemaName}].[{tableName}] WHERE {whereExpr}";

            var sqlResult = ExecuteSQL(sqlCommand, connParams);
            if (sqlResult.Messages.Any())
                return Result<bool>.Fail(sqlResult.Messages);

            return Result<bool>.Success(true);
        }
        #endregion

        #region Table
        public bool CreateTable(CreateTable table)
        {
            var columns = table.Columns.Select(x => $"{x.Name} {x.SqlType}");
            var columnsExpr = string.Join(", ", columns);

            var sqlCommand = $"CREATE TABLE [{table.TableName}] ({columnsExpr})";

            var connection = GetAdminConnParams(table.DatabaseName);
            var sqlResult = ExecuteSQL(sqlCommand, connection);

            //if (sqlResult.Messages.Any()) - CreateDatabase treba da uloguje gresku + Messages
            //    Log

            return GetTableNames(connection)
                    .Where(tn => string.Equals(tn, $"dbo.{table.TableName}", StringComparison.OrdinalIgnoreCase))
                    .Any();
        }
        public IEnumerable<string> CreateTables(IEnumerable<CreateTable> tables)
        {
            var successfullyCreated = new List<string>();

            foreach (var table in tables)
                if (CreateTable(table))
                    successfullyCreated.Add(table.TableName);

            return successfullyCreated;
        }
        public bool DeleteTableIfExists(string tableName, string databaseName)
        {
            var sqlCommand = $"DROP TABLE [{tableName}]";

            var connection = GetAdminConnParams(databaseName);
            var sqlResult = ExecuteSQL(sqlCommand, connection);

            //if (sqlResult.Messages.Any()) - CreateDatabase treba da uloguje gresku + Messages
            //    Log

            return !GetTableNames(connection)
                    .Where(tn => string.Equals(tn, $"dbo.{tableName}", StringComparison.OrdinalIgnoreCase))
                    .Any();
        }
        public QueryExecuteRS ReadTable(string schemaName, string tableName, ConnectionParams connParams)
        {
            var sqlCommand = $"SELECT * FROM [{schemaName}].[{tableName}]";
            return ExecuteSQL(sqlCommand, connParams);
        }
        public int Count(string schemaName, string tableName, List<CellDTO> recordPrevious, ConnectionParams connParams)
        {
            if (!recordPrevious.Any())
                return -1;

            var columnValuesWhere = recordPrevious.Select(x =>
                x.Value.ToLower().Equals("null") ? $"[{x.ColumnName}] IS NULL" : $"[{x.ColumnName}] = {_msSqlQueryBuilder.ToQuery(x.ColumnType, x.Value)}"
            );
            var whereExpr = string.Join(" and ", columnValuesWhere);

            var sqlCommand = $"SELECT COUNT(*) FROM [{schemaName}].[{tableName}] WHERE {whereExpr}";
            var sqlResult = ExecuteSQL(sqlCommand, connParams);

            var count = sqlResult.Tables.First().Rows.First().First().ToString();
            return int.Parse(count);
        }
        #endregion

        #region Database
        public bool CreateDatabase(string dbName, bool contained = false)
        {
            var sqlCommand = $"CREATE DATABASE [{dbName}]";
            if (contained)
                sqlCommand += " CONTAINMENT = PARTIAL";

            var sqlResult = ExecuteSQLAsAdmin(sqlCommand);

            //if (sqlResult.Messages.Any()) - CreateDatabase treba da uloguje gresku + Messages
            //    Log

            return DatabaseExists(dbName);
        }
        public bool CloneDatabase(string originDbName, string destDbName, bool failIfExists)
        {
            if (DatabaseExists(destDbName))
                return !failIfExists;

            var backupDestination = $"{AppContext.BaseDirectory}AppData\\{destDbName}_Temp.bak";

            var backupToDiskCommand = $"BACKUP DATABASE [{originDbName}]  TO DISK = '{backupDestination}' WITH FORMAT, COPY_ONLY";
            var sqlResult = ExecuteSQLAsAdmin(backupToDiskCommand);
            if (!File.Exists(backupDestination) || !sqlResult.Messages.Where(x => x.StartsWith("BACKUP DATABASE successfully")).Any())
            {
                // LOG error(messages)
                return false;
            }

            var restoreFromDiskCommand1 = $"RESTORE FILELISTONLY FROM DISK = '{backupDestination}'";
            sqlResult = ExecuteSQLAsAdmin(restoreFromDiskCommand1);
            if (sqlResult.Messages.Any())
            {
                // LOG error(messages)
                File.Delete(backupDestination);
                return false;
            }

            var logicalMDF = sqlResult.Tables.Single().Rows[0][0].ToString();
            var logicalLDF = sqlResult.Tables.Single().Rows[1][0].ToString();

            var backupDestinationTrimmed = backupDestination.Replace("_Temp.bak", "");
            var restoreFromDiskCommand2 = $"RESTORE DATABASE [{destDbName}] FROM DISK = '{backupDestination}' WITH RECOVERY, " +
                                          $"MOVE '{logicalMDF}' TO '{backupDestinationTrimmed}.mdf', " +
                                          $"MOVE '{logicalLDF}' TO '{backupDestinationTrimmed}._log.ldf'";
            sqlResult = ExecuteSQLAsAdmin(restoreFromDiskCommand2);
            if (!sqlResult.Messages.Where(x => x.StartsWith("RESTORE DATABASE successfully")).Any())
            {
                // LOG error(messages)
                File.Delete(backupDestination);
                return false;
            };

            try
            {
                File.Delete(backupDestination);
            }
            catch (Exception ex)
            {
                //kloniranje je uspelo, cleanup nije - ako bih ovde vratio false imao bih bazu koja "visi" na sqlServeru jer se ona kasnije ne bi registrovala u bazu
                //LOG(ex) - vodi racuna o kruznim referencama kod serijalizacije (vidi kako si resio u midleware-u)
            }

            return DatabaseExists(destDbName);
        }
        public IEnumerable<string> CloneDatabases(string originDbName, IEnumerable<string> destDbNames, bool failIfExists)
        {
            var successfullyCloned = new List<string>();

            foreach (var destName in destDbNames)
                if (CloneDatabase(originDbName, destName, failIfExists))
                    successfullyCloned.Add(destName);

            return successfullyCloned;
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

            var sqlQuery = $"ALTER DATABASE [{name}] SET single_user WITH ROLLBACK IMMEDIATE";
            var sqlResult = ExecuteSQLAsAdmin(sqlQuery);

            sqlQuery = $"DROP DATABASE [{name}]";
            sqlResult = ExecuteSQLAsAdmin(sqlQuery);

            return !DatabaseExists(name);
        }
        #endregion

        #region SQL_User_Manager
        public IEnumerable<string> GetDatabaseUsers(string dbName)
        {
            var sqlCommand = "SELECT [name] FROM [sys].[database_principals] WHERE [type] = N'S'";

            var sqlResult = ExecuteSQLAsAdmin(sqlCommand, dbName);

            return sqlResult
                    .Tables[0]
                    .Rows
                    .Select(x => x[0].ToString());
        }
        public bool UserExists(string username, string dbName)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(dbName))
                return false;

            return GetDatabaseUsers(dbName)
                    .Where(x => x.ToLower().Equals(username.ToLower()))
                    .Any();
        }

        public bool CreateServerLogin(string login, string password)
        {
            var sqlCommand = $"CREATE LOGIN [{login}] " +
                              $"WITH PASSWORD='{password}', " +
                              "DEFAULT_DATABASE=[master], " +
                              "DEFAULT_LANGUAGE=[us_english], " +
                              "CHECK_EXPIRATION=OFF, " +
                              "CHECK_POLICY=ON";

            var sqlResult = ExecuteSQLAsAdmin(sqlCommand);
            return sqlResult.NoMessages;
        }
        public bool CreateDbUserFromLogin(string userLogin, string dbName)
        {
            var sqlCommand = $"CREATE USER [{userLogin}] FOR LOGIN [{userLogin}] WITH DEFAULT_SCHEMA=[dbo] " +
                             $"EXEC sp_addrolemember 'db_datawriter' , '{userLogin}' " +
                             $"EXEC sp_addrolemember 'db_datareader' , '{userLogin}'";

            var sqlResult = ExecuteSQLAsAdmin(sqlCommand, dbName);
            return sqlResult.NoMessages;
        }
        public bool CreateDbUserContained(string userName, string password, string dbName)
        {
            var sqlCommand = $"CREATE USER [{userName}]  WITH PASSWORD = '{password}';";

            var sqlResult = ExecuteSQLAsAdmin(sqlCommand, dbName);
            return sqlResult.NoMessages;
        }
        public bool DeleteDbUser(string userName, string dbName)
        {
            var sqlCommand = $"DROP USER [{userName}]";

            var sqlResult = ExecuteSQLAsAdmin(sqlCommand, dbName);
            return sqlResult.NoMessages;
        }
        public bool DeleteServerLogin(string userLogin)
        {
            var sqlCommand = $"DROP LOGIN [{userLogin}]";

            var sqlResult = ExecuteSQLAsAdmin(sqlCommand);
            return sqlResult.NoMessages;
        }

        public bool AssignReadonly(string userName, string dbName)
        {
            var sqlCommand = $"GRANT SELECT TO [{userName}]";

            var sqlResult = ExecuteSQLAsAdmin(sqlCommand, dbName);
            return sqlResult.NoMessages;
        }
        public bool AssignCRUD(string userName, string dbName)
        {
            var sqlCommand = $"GRANT INSERT TO [{userName}];" +
                             $"GRANT SELECT TO [{userName}];" +
                             $"GRANT UPDATE TO [{userName}];" +
                             $"GRANT DELETE TO [{userName}];";

            var sqlResult = ExecuteSQLAsAdmin(sqlCommand, dbName);
            return sqlResult.NoMessages;
        }
        public bool AssignReadonly(string userName, string dbName, string tableName, string schemaName = null)
        {
            if (string.IsNullOrEmpty(schemaName))
                schemaName = Constants.DefaultSchemaName;

            var sqlCommand = $"GRANT SELECT ON [{schemaName}].[{tableName}] TO [{userName}]" +
                             $"GRANT VIEW DEFINITION ON SCHEMA :: [{schemaName}] TO [{userName}];";

            var sqlResult = ExecuteSQLAsAdmin(sqlCommand, dbName);
            return sqlResult.NoMessages;
        }
        public bool AssignCRUD(string userName, string dbName, string tableName, string schemaName = null)
        {
            if (string.IsNullOrEmpty(schemaName))
                schemaName = Constants.DefaultSchemaName;

            var sqlCommand = $"GRANT INSERT ON [{schemaName}].[{tableName}] TO [{userName}];" +
                             $"GRANT SELECT ON [{schemaName}].[{tableName}] TO [{userName}];" +
                             $"GRANT UPDATE ON [{schemaName}].[{tableName}] TO [{userName}];" +
                             $"GRANT DELETE ON [{schemaName}].[{tableName}] TO [{userName}];" + 
                             $"GRANT VIEW DEFINITION ON SCHEMA :: [{schemaName}] TO [{userName}];";

            var sqlResult = ExecuteSQLAsAdmin(sqlCommand, dbName);
            return sqlResult.NoMessages;
        }
        #endregion
    }
}
