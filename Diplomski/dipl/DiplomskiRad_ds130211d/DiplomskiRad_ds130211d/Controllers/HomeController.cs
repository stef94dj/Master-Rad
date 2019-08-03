using DiplomskiRad_ds130211d.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static DiplomskiRad_ds130211d.Models.KorisnikModel;

namespace DiplomskiRad_ds130211d.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult SignIn(PrijavaModel kredencijali)
        {
            System.Threading.Thread.Sleep(1500);
            List<KorisnikModel> korisnici = null;
            using (var db = new KorisnikContext())
            {
                korisnici = db.Korisnici.Where(x => x.KorisnickoIme.Equals(kredencijali.KorisnickoIme) &&
                x.Lozinka.Equals(kredencijali.Lozinka)).ToList();
            }

            if (korisnici.Count == 0)
            {
                return Json(new { msg = "Pogrešno korisničko ime ili lozinka" });
            }

            if (korisnici[0].JeAdministrator)
            {
                Session["KorisnickoIme"] = korisnici[0].KorisnickoIme;
                return Json(new { result = "Redirect", url = Url.Action("ShowHome", "Admin") });
            }
            
            Session["KorisnickoIme"] = korisnici[0].KorisnickoIme;
            return Json(new { result = "Redirect", url = Url.Action("ShowHome", "Student") });

        }

        private void DeleteUserDatabase()
        {
            string databaseName = Session["sessionDbName"]?.ToString();
            if (!string.IsNullOrEmpty(databaseName))
            {
                DatabaseController.DeleteDatabase(databaseName);
            }
        }
        public ActionResult SignOut()
        {
            DeleteUserDatabase();
            Session.Abandon();
            return View("Index");
        }

        public void BrowserClose()
        {
            DeleteUserDatabase();
            Session.Abandon();
        }

        public string UnesiStudentaIAdmina()
        {
            using (var db = new KorisnikModel.KorisnikContext())
            {
                var student = new KorisnikModel
                {
                    KorisnickoIme = "student1",
                    Lozinka = "test12",
                    Ime = "Stefan",
                    Prezime = "Djordjevic",
                    BrojIndeksa = "2013/0211",
                    JeAdministrator = false
                };

                db.Korisnici.Add(student);

                var admin = new KorisnikModel
                {
                    KorisnickoIme = "admin1",
                    Lozinka = "test12",
                    Ime = "Milos",
                    Prezime = "Cvetanovic",
                    BrojIndeksa = "",
                    JeAdministrator = true
                };

                db.Korisnici.Add(admin);
                db.SaveChanges();
            }
            return "zavrsio";
        }
    }
}