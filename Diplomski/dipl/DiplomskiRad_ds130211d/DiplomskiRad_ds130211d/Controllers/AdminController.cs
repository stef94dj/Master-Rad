using DiplomskiRad_ds130211d.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiplomskiRad_ds130211d.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult ShowHome()
        {
            string t = Session["KorisnickoIme"]?.ToString();
            if (string.IsNullOrEmpty(t))
            {
                t = "prazanString";
            }
            //return View("HomeAdminView", (object)t);

            AdminTableViewModel data = new AdminTableViewModel();
            string korisnicko = Session["KorisnickoIme"]?.ToString();
            KorisnikModel korisnik = null;
            using (var db = new KorisnikModel.KorisnikContext())
            {
                korisnik = (db.Korisnici.Where(x => x.KorisnickoIme.Equals(korisnicko)).ToList())[0];
            }
            data.ImePrezime = korisnik.Ime + " " + korisnik.Prezime;
            data.tabela = DatabaseController.GetTable("BazaAplikacije", "Korisnik");

            data.tabela.Rows.RemoveAll(x => x[6].Equals("True")); //izbaci administratore
            return View("HomeAdminView", data);
        }

        public ActionResult ShowDatabases()
        {
            string t = Session["KorisnickoIme"]?.ToString();
            if (string.IsNullOrEmpty(t))
            {
                t = "prazanString";
            }
            //return View("HomeAdminView", (object)t);

            AdminTableViewModel data = new AdminTableViewModel();
            string korisnicko = Session["KorisnickoIme"]?.ToString();
            KorisnikModel korisnik = null;
            using (var db = new KorisnikModel.KorisnikContext())
            {
                korisnik = (db.Korisnici.Where(x => x.KorisnickoIme.Equals(korisnicko)).ToList())[0];
            }
            data.ImePrezime = korisnik.Ime + " " + korisnik.Prezime;
            data.tabela = DatabaseController.GetTable("BazaAplikacije", "Baza"); 

            return View("AdminBazeView", data);
        }

        public ActionResult ShowAdd()
        {
            AdminTableViewModel data = new AdminTableViewModel();
            string korisnicko = Session["KorisnickoIme"]?.ToString();
            KorisnikModel korisnik = null;
            using (var db = new KorisnikModel.KorisnikContext())
            {
                korisnik = (db.Korisnici.Where(x => x.KorisnickoIme.Equals(korisnicko)).ToList())[0];
            }
            data.ImePrezime = korisnik.Ime + " " + korisnik.Prezime;
            return View("AdminDodajView", data);
        }

        public ActionResult UploadDatabase(HttpPostedFileBase fileBackup, HttpPostedFileBase fileImage, string dbDesc)
        {
            string dbName = "";
            string pathForImageSave = "";
            if (fileBackup != null && fileBackup.ContentLength > 0)
            {
                var fileName = Path.GetFileName(fileBackup.FileName);
                dbName = fileName;
                dbName = dbName.Substring(0, dbName.Length - 4);
                var path = Path.Combine(Server.MapPath("~/Content"), fileName);

                //override (if there is a file with the same name)
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                // store the backup inside ~/App_Data/ folder
                fileBackup.SaveAs(path);

                //restore the backup to server
                string connetionString = "Persist Security Info = False; Integrated Security = true; Initial Catalog = BazaAplikacije; server = (LocalDb)\\MSSQLLocalDB";
                using (SqlConnection cnn = new SqlConnection(connetionString))
                {
                    try
                    {
                        cnn.Open();
                        using (SqlCommand cmnd = cnn.CreateCommand())
                        {
                            cmnd.CommandText = "RESTORE FILELISTONLY FROM DISK = '" + path + "'";
                            SqlDataReader dr = cmnd.ExecuteReader();
                            dr.Read();
                            string logicalMDF = dr.GetString(0);
                            dr.Read();
                            string logicalLDF = dr.GetString(0);
                            dr.Close();

                            string pathWithoutExtension = pathForImageSave  = path.Substring(0, path.Length - 4);
                            string fileNameWithoutExtension = fileName.Substring(0, fileName.Length - 4);
                            cmnd.CommandText = "RESTORE DATABASE " + fileNameWithoutExtension + " FROM DISK = '" + path + "'" +
                                                  " WITH RECOVERY," +
                                                  " MOVE '" + logicalMDF + "' TO '" + pathWithoutExtension + ".mdf'," +
                                                  " MOVE '" + logicalLDF + "' TO '" + pathWithoutExtension + "_log.ldf'; ";
                            cmnd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("DatabaseController GetNamesOfDatabases: Can not open connection ! " + ex.Message);
                    }
                }
                //delete the backup
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }

            string imageName = "";
            if (fileImage != null && fileImage.ContentLength > 0)
            {
                imageName = Path.GetFileName(fileImage.FileName);

                //override (if there is a file with the same name)
                if (System.IO.File.Exists(pathForImageSave + "_diagram.png"))
                {
                    System.IO.File.Delete(pathForImageSave + "_diagram.png");
                }

                // store the backup inside ~/App_Data/ folder
                fileImage.SaveAs(pathForImageSave + "_diagram.png");
            }


            using (var db = new BazaModel.BazaContext())
            {
                var novaBaza = new BazaModel
                {
                   NazivBaze = dbName,
                   OpisBaze = dbDesc,
                   NazivFajlaSlike = dbName + "_diagram.png",
                };

                db.Baze.Add(novaBaza);
                db.SaveChanges();
            }


            return RedirectToAction("ShowAdd");
        }

        public ActionResult UploadStudent(string un, string pass, string fn, string ln, string ind) 
        {
            using (var db = new KorisnikModel.KorisnikContext())
            {
                var novStudent = new KorisnikModel
                {
                    KorisnickoIme = un,
                    Lozinka = pass,
                    Ime = fn,
                    Prezime = ln,
                    BrojIndeksa = ind,
                    JeAdministrator = false
                };

                db.Korisnici.Add(novStudent);
                db.SaveChanges();
            }
            return RedirectToAction("ShowAdd");
        }

    }
}