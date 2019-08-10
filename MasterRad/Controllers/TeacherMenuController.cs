using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MasterRad.Models;
using MasterRad.Models.ViewModels;
using MasterRad.Repositories;
using MasterRad.DTOs;
using MasterRad.Extensions;

namespace MasterRad.Controllers
{
    public class TeacherMenuController : Controller
    {
        public IActionResult Templates()
        {
            return View();
        }

        public IActionResult Tasks()
        {
            return View();
        }
    }
}
