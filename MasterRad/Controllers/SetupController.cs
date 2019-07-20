using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MasterRad.Models;

namespace MasterRad.Controllers
{
    public class SetupController : Controller
    {
        public IActionResult MyDatabases()
        {
            return View();
        }

        public IActionResult MyDatabase()
        {
            return View();
        }
    }
}
