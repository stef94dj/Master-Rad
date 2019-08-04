using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiplomskiRad_ds130211d.Models
{
    public class StudentIzborBazeView
    {
        public StudentIzborBazeView()
        {
            Baze = new List<BazaModel>();
        }
        public List<BazaModel> Baze { get; set; }
        public string ImePrezime { get; set; }
    }
}