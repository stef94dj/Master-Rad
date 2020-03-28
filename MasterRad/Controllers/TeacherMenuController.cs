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
using Microsoft.Identity.Web;

namespace MasterRad.Controllers
{
    public class TeacherMenuController : Controller
    {
        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserRead, Constants.ScopeUserReadBasicAll })]
        public IActionResult Templates()
            => View();

        public IActionResult Tasks()
            => View();

        public IActionResult SynthesisTests()
            => View();

        public IActionResult AnalysisTests()
            => View();
    }
}
