using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace DiplomskiRad_ds130211d.Models
{
    [Table("Baza")]
    public class BazaModel
    {
        public int Id { get; set; }
        public string NazivBaze { get; set; }
        public string OpisBaze { get; set; }
        public string NazivFajlaSlike { get; set; }

        public class BazaContext : DbContext
        {
            public BazaContext() : base("BazaAplikacije") { }
            public DbSet<BazaModel> Baze { get; set; }
        }
    }
}