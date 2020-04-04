using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MasterRad.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using WebApp_OpenIDConnect_DotNet.Services;
using Graph = Microsoft.Graph;
using Microsoft.Extensions.Options;
using MasterRad.Models.Configuration;

namespace MasterRad.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
            => View();


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
