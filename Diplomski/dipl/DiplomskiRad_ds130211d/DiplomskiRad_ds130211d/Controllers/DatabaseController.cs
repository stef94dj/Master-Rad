using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Data;
using DiplomskiRad_ds130211d.Models;
using System.IO;
using static DiplomskiRad_ds130211d.Models.DatabaseViewModel.Table;
using static DiplomskiRad_ds130211d.Models.KorisnikModel;

namespace DiplomskiRad_ds130211d.Controllers
{
    public class DatabaseController : Controller
    {
        public static List<string> GetNamesOfDatabases()
        {
            List<string> names = new List<string>();
            string connetionString = "Persist Security Info = False; Integrated Security = true; Initial Catalog = BazaAplikacije; server = (LocalDb)\\MSSQLLocalDB";
            using (SqlConnection cnn = new SqlConnection(connetionString))
            {
                try
                {
                    cnn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases", cnn))
                    {
                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                names.Add(dr[0].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("DatabaseController GetNamesOfDatabases: Can not open connection ! " + ex.Message);
                }
            }
            return names;
        }
        public static DatabaseViewModel GetDatabaseViewModel(string databaseName)
        {
            DatabaseViewModel dbvm = new DatabaseViewModel();
            dbvm.Name = databaseName;

            string connetionString = "Persist Security Info = False; Integrated Security = true; Initial Catalog = " + databaseName + "; server = (LocalDb)\\MSSQLLocalDB";
            using (SqlConnection cnn = new SqlConnection(connetionString))
            {
                try
                {
                    cnn.Open();

                    //get table names
                    DataTable schema = cnn.GetSchema("Tables");
                    List<string> TableNames = new List<string>();
                    foreach (DataRow row in schema.Rows)
                    {
                        TableNames.Add(row[2].ToString());
                    }

                    //create tables
                    foreach (string tn in TableNames)
                    {
                        DatabaseViewModel.Table newTable = new DatabaseViewModel.Table();

                        newTable.Name = tn;

                        using (SqlCommand cmnd = cnn.CreateCommand())
                        {
                            cmnd.CommandText = "SELECT * FROM [" + tn + "]";
                            DataTable dt = new DataTable();
                            dt.Load(cmnd.ExecuteReader());

                            //set collumn names 
                            foreach (DataColumn column in dt.Columns)
                            {
                                newTable.NamesOfCollumns.Add(column.ColumnName);
                            }

                            //set rows
                            foreach (DataRow row in dt.Rows)
                            {
                                List<string> newRow = new List<string>();
                                foreach (string colName in newTable.NamesOfCollumns)
                                {
                                    newRow.Add(row[colName].ToString());
                                }
                                newTable.Rows.Add(newRow);
                            }
                        }

                        dbvm.Tables.Add(newTable);
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("DatabaseController GetDatabaseViewModel: Can not open connection ! " + ex.Message);
                }
            }

            return dbvm;
        }
        public static bool CreateDatabaseCopy(string imeIzabraneBaze, string imeNoveBaze)
        {
            string connetionString = "Persist Security Info = False; Integrated Security = true; Initial Catalog = BazaAplikacije; server = (LocalDb)\\MSSQLLocalDB";
            string projectFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string appDataPath = projectFolder + "App_Data\\";
            string check = appDataPath + imeNoveBaze;
            if (!(System.IO.File.Exists(check + ".mdf") || System.IO.File.Exists(check + "_log.ldf")))
            {
                using (SqlConnection cnn = new SqlConnection(connetionString))
                {
                    try
                    {
                        cnn.Open();
                        using (SqlCommand cmnd = cnn.CreateCommand())
                        {
                            //backup to disk
                            cmnd.CommandText = "BACKUP DATABASE " + imeIzabraneBaze +
                                               " TO DISK = '" + appDataPath + imeNoveBaze + "_Temp.bak' WITH FORMAT, COPY_ONLY";
                            cmnd.ExecuteNonQuery();

                            //restore from disk
                            cmnd.CommandText = "RESTORE FILELISTONLY FROM DISK = '" + appDataPath + imeNoveBaze + "_Temp.bak'";
                            SqlDataReader dr = cmnd.ExecuteReader();
                            dr.Read();
                            string logicalMDF = dr.GetString(0);
                            dr.Read();
                            string logicalLDF = dr.GetString(0);
                            dr.Close();

                            cmnd.CommandText = "RESTORE DATABASE " + imeNoveBaze + " FROM DISK = '" + appDataPath + imeNoveBaze + "_Temp.bak'" +
                                                  " WITH RECOVERY," +
                                                  " MOVE '" + logicalMDF + "' TO '" + appDataPath + imeNoveBaze + ".mdf'," +
                                                  " MOVE '" + logicalLDF + "' TO '" + appDataPath + imeNoveBaze + "_log.ldf'; ";
                            cmnd.ExecuteNonQuery();

                            //delete backup from disk
                            if ((System.IO.File.Exists(appDataPath + imeNoveBaze + "_Temp.bak")))
                            {
                                System.IO.File.Delete(appDataPath + imeNoveBaze + "_Temp.bak");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("DatabaseController GetNamesOfDatabases: Can not open connection ! " + ex.Message);
                        return false;
                    }
                }
            }

            return true;
        }
        public static void DeleteDatabase(string databaseName)
        {
            string connetionString = "Persist Security Info = False; Integrated Security = true; Initial Catalog = master; server = (LocalDb)\\MSSQLLocalDB";
            using (SqlConnection cnn = new SqlConnection(connetionString))
            {
                try
                {
                    cnn.Open();

                    using (SqlCommand cmnd = cnn.CreateCommand())
                    {
                        //close all conections on database being deleted
                        cmnd.CommandText = "alter database [" + databaseName + "] set single_user with rollback immediate";
                        cmnd.ExecuteNonQuery();

                        //delete the database
                        cmnd.CommandText = "USE master DROP DATABASE IF EXISTS " + databaseName;
                        cmnd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("DatabaseController DeleteDatabase: Exception ! " + ex.Message);
                }
            }
        }

        public static StudentBazaQryResultModel ExecuteSelect(string qry, string dbName)
        {

            DatabaseViewModel.Table resultTable = new DatabaseViewModel.Table();
            string resultMessage = "Execute Select - USPESNO";

            string connetionString = "Persist Security Info = False; Integrated Security = true; Initial Catalog = " + dbName + "; server = (LocalDb)\\MSSQLLocalDB";
            using (SqlConnection cnn = new SqlConnection(connetionString))
            {
                try
                {
                    cnn.Open();
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = qry;
                        DataTable dt = new DataTable();
                        dt.Load(cmd.ExecuteReader());

                        //set collumn names 
                        foreach (DataColumn column in dt.Columns)
                        {
                            resultTable.NamesOfCollumns.Add(column.ColumnName);
                        }

                        //set rows
                        foreach (DataRow row in dt.Rows)
                        {
                            List<string> newRow = new List<string>();
                            foreach (string colName in resultTable.NamesOfCollumns)
                            {
                                newRow.Add(row[colName].ToString());
                            }
                            resultTable.Rows.Add(newRow);
                        }

                        if ((dt?.Columns?.Count ?? 0) == 0)
                        {
                            resultMessage = "Execute Select - NEUSPESNO: " + "Proverite da li je u pitanju SELECT upit.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("DatabaseController ExecuteSelect: Greska ! " + ex.Message);
                    resultMessage = "Execute Select - NEUSPESNO: " + ex.Message;
                }

                return new StudentBazaQryResultModel { message = resultMessage, table = resultTable };
            }
        }

        public static StudentBazaQryResultModel ExecuteEdit(string qry, string dbName)
        {
            string resultMessage = "Execute Edit, Update ili Delete - USPESNO";

            string connetionString = "Persist Security Info = False; Integrated Security = true; Initial Catalog = " + dbName + "; server = (LocalDb)\\MSSQLLocalDB";
            using (SqlConnection cnn = new SqlConnection(connetionString))
            {
                try
                {
                    cnn.Open();
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = qry;
                        int rows = cmd.ExecuteNonQuery();
                        if (rows != -1)
                        {
                            resultMessage += ". Rows affected: " + rows;
                        }
                        else
                        {
                            resultMessage = "Execute Edit, Update ili Delete - NEUSPESNO: " + "Proverite da li je u pitanju INSERT, UPDATE ili DELETE upitu.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("DatabaseController ExecuteSelect: Greska ! " + ex.Message);
                    resultMessage = "Execute Edit, Update ili Delete - NEUSPESNO: " + ex.Message;
                }

                return new StudentBazaQryResultModel { message = resultMessage, table = null };
            }
        }

        public static List<string> GetNamesOfTables(string imeBaze)
        {
            List<string> result = new List<string>();
            string connetionString = "Persist Security Info = False; Integrated Security = true; Initial Catalog = " + imeBaze + "; server = (LocalDb)\\MSSQLLocalDB";
            using (SqlConnection cnn = new SqlConnection(connetionString))
            {
                try
                {
                    cnn.Open();

                    DataTable schema = cnn.GetSchema("Tables");

                    foreach (DataRow row in schema.Rows)
                    {
                        result.Add(row[2].ToString());
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("DatabaseController GetNamesOfTables: Exception uhvacen ! " + ex.Message);
                }
            }
            return result;
        }

        public static AdminTableViewModel.Table GetTable(string databaseName, string tableName)
        {
            AdminTableViewModel.Table result = new AdminTableViewModel.Table();

            result.Name = tableName;
            string connetionString = "Persist Security Info = False; Integrated Security = true; Initial Catalog = " + databaseName + "; server = (LocalDb)\\MSSQLLocalDB";
            using (SqlConnection cnn = new SqlConnection(connetionString))
            {
                using (SqlCommand cmnd = cnn.CreateCommand())
                {
                    cnn.Open();
                    cmnd.CommandText = "SELECT * FROM [" + tableName + "]";
                    DataTable dt = new DataTable();
                    dt.Load(cmnd.ExecuteReader());

                    //set collumn names 
                    foreach (DataColumn column in dt.Columns)
                    {
                        result.NamesOfCollumns.Add(column.ColumnName);
                    }

                    //set rows
                    foreach (DataRow row in dt.Rows)
                    {
                        List<string> newRow = new List<string>();
                        foreach (string colName in result.NamesOfCollumns)
                        {
                            newRow.Add(row[colName].ToString());
                        }
                        result.Rows.Add(newRow);
                    }
                }
            }

            return result;
        }
    }
}