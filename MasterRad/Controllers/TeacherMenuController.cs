using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MasterRad.Models;
using MasterRad.Models.ViewModels;
using MasterRad.Repositories;
using MasterRad.DTO;
using MasterRad.Extensions;
using Microsoft.Identity.Web;
using MasterRad.Models.Configuration;
using Microsoft.Graph;
using WebApp_OpenIDConnect_DotNet.Services;
using Microsoft.Extensions.Options;

namespace MasterRad.Controllers
{
    public class TeacherMenuController : BaseController
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly WebOptions _webOptions;

        public TeacherMenuController
        (
             ITokenAcquisition tokenAcquisition,
             IOptions<WebOptions> webOptions
        )
        {
            _tokenAcquisition = tokenAcquisition;
            _webOptions = webOptions.Value;
        }

        [Route("Templates")]
        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserReadBasicAll })]
        public async Task<IActionResult> TemplatesAsync()
        {
            var scopes = new[] { Constants.ScopeUserReadBasicAll };
            await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);

            return View();
        }

        public IActionResult Tasks()
            => View();

        public IActionResult SynthesisTests()
            => View();

        public IActionResult AnalysisTests()
            => View();
    }
}
