using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace DiplomskiRad_ds130211d.Models
{
    public class StudentBazaViewModel
    {

        public StudentBazaViewModel()
        {
            ImenaTabela = new List<string>();
        }
        public string ImePrezime { get; set; }
        public string ImeBaze { get; set; }
        public List<string> ImenaTabela { get; set; }

    }
}