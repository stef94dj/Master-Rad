using DiplomskiRad_ds130211d.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiplomskiRad_ds130211d.Controllers
{
    public class StudentController : Controller
    {
        public ActionResult ShowHome()
        {
            StudentIzborBazeView data = new StudentIzborBazeView();

            List<BazaModel> baze = null;
            using (var db = new BazaModel.BazaContext())
            {
                baze = (db.Baze).ToList();
            }
            data.Baze = baze;

            string korisnicko = Session["KorisnickoIme"]?.ToString();
            KorisnikModel korisnik = null;
            using (var db = new KorisnikModel.KorisnikContext())
            {
                korisnik = (db.Korisnici.Where(x => x.KorisnickoIme.Equals(korisnicko)).ToList())[0];
            }
            data.ImePrezime = korisnik.Ime + " " + korisnik.Prezime;

            return View("HomeStudentView", data);
        }

        public ActionResult OtvoriBazu(string imeIzabraneBaze)
        {
            List<string> namesOfDatabases = DatabaseController.GetNamesOfDatabases();

            bool postoji = false;
            foreach (string dbn in namesOfDatabases)
            {
                if (dbn.Equals(imeIzabraneBaze))
                {
                    postoji = true;
                    break;
                }
            }

            if (!postoji)
            {
                return View("MyError", (object)"Izabrana baza ne postoji");
            }

            string trenutnaBaza = Session["sessionDbName"]?.ToString();
            if (!string.IsNullOrEmpty(trenutnaBaza) && 
                !trenutnaBaza.Equals(imeIzabraneBaze + "_" +Session["KorisničkoIme"]?.ToString()))
            {
                DatabaseController.DeleteDatabase(trenutnaBaza);
            }

            string korisnicko = Session["KorisnickoIme"]?.ToString();
            string imeNoveBaze = imeIzabraneBaze + "_" + korisnicko;
            Session["sessionDbName"] = imeNoveBaze;
            bool dbCopyResult = DatabaseController.CreateDatabaseCopy(imeIzabraneBaze, imeNoveBaze);
            if (dbCopyResult == false)
            {
                return View("MyError", (object)"Greska pri kreiranju kopije baze");
            }

            StudentBazaViewModel data = new StudentBazaViewModel();

            data.ImeBaze = imeIzabraneBaze;
            
            KorisnikModel korisnik = null;
            using (var db = new KorisnikModel.KorisnikContext())
            {
                korisnik = (db.Korisnici.Where(x => x.KorisnickoIme.Equals(korisnicko)).ToList())[0];
            }
            data.ImePrezime = korisnik.Ime + " " + korisnik.Prezime;

            List<string> imenaKolona = DatabaseController.GetNamesOfTables(imeNoveBaze);
            foreach (string imeKolone in imenaKolona)
            {
                if (!"__MigrationHistory".Equals(imeKolone))
                {
                    data.ImenaTabela.Add(imeKolone);
                }
            }

            return View("StudentBazaView", data);
        }
        public ActionResult ExecuteSelectQuery(StudentBazaUpitModel qry)
        {
            System.Threading.Thread.Sleep(1500);

            string qryText = qry.QueryText;
            string dbName = Session["sessionDbName"]?.ToString();

            StudentBazaQryResultModel qryResult = null;
            if (!string.IsNullOrEmpty(qryText) && !string.IsNullOrEmpty(dbName))
            {
                qryResult = DatabaseController.ExecuteSelect(qryText, dbName);
            }

            return Json(qryResult);
        }
        public ActionResult ExecuteEditQuery(StudentBazaUpitModel qry)
        {
            System.Threading.Thread.Sleep(1500);

            string qryText = qry.QueryText;
            string dbName = Session["sessionDbName"]?.ToString();

            StudentBazaQryResultModel qryResult = null;
            if (!string.IsNullOrEmpty(qryText) && !string.IsNullOrEmpty(dbName))
            {
                qryResult = DatabaseController.ExecuteEdit(qryText, dbName);
            }

            return Json(qryResult);
        }

    }
}