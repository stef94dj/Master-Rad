using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DiplomskiRad_ds130211d.Models
{
    [Table("Korisnik")]
    public class KorisnikModel
    {
        [Key]
        public int Id { get; set; }

        [Index(IsUnique = true)]
        [StringLength(100)]
        public string KorisnickoIme { get; set; }
        public string Lozinka { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string BrojIndeksa { get; set; }
        public bool JeAdministrator { get; set; }

        public class KorisnikContext : DbContext
        {
            public KorisnikContext() : base("BazaAplikacije") { }
            public DbSet<KorisnikModel> Korisnici { get; set; }
        }

    }
}