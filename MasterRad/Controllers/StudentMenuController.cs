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
    public class StudentMenuController : Controller
    {
        public IActionResult Exercises()
            => View();

        public IActionResult Tests()
            => View();

        public IActionResult StartTest()
            => View("~/Views/StudentMenu/Exercises.cshtml");

    }
}
