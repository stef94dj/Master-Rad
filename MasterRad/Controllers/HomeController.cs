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
        private readonly ILogger<HomeController> _logger;
        readonly ITokenAcquisition _tokenAcquisition;
        readonly WebOptions _webOptions;

        public HomeController
        (
            ILogger<HomeController> logger,
            ITokenAcquisition tokenAcquisition,
            IOptions<WebOptions> webOptions
        )
        {
            _tokenAcquisition = tokenAcquisition;
            _webOptions = webOptions.Value;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserRead, Constants.ScopeUserReadBasicAll })] 
        public async Task<IActionResult> PrivacyAsync()
        {
            // Initialize the GraphServiceClient. 
            Graph::GraphServiceClient graphClient = GetGraphServiceClient(new[] { Constants.ScopeUserRead, Constants.ScopeUserReadBasicAll });


            //first 5 pages of users with name starting with "Stefan"
            var nameStartsWith = "Stefan";
            var recordsInOnePage = 2;

            var users = await graphClient.Users
                                         .Request()
                                         .Filter($"startswith(givenName,+'{nameStartsWith}')")
                                         .Top(recordsInOnePage)
                                         .GetAsync();

            //user 2 data
            string page1_user_DisplayName = users[1].DisplayName;
            string page1_user_Mail = users[1].Mail;
            string page1_user_OnPremisesSamAccountName = users[1].OnPremisesSamAccountName;

            //SERVER DRIVEN PAGINATION - BE IS REQUESTING next page)
            //users = await users.NextPageRequest.GetAsync();
            //string page2_user_DisplayName = users[2].DisplayName;
            //string page2_user_Mail = users[2].Mail;
            //string page2_user_OnPremisesSamAccountName = users[2].OnPremisesSamAccountName;

            ////page 4 (CLIENT DRIVE PAGINATION - Browser IS REQUESTING page 4) - NOT SUPPORTED, endpoint doesn't support skip
            //var requestedPage = 4;
            //users = await graphClient.Users
            //                         .Request()
            //                         .Filter($"startswith(givenName,+'{nameStartsWith}')")
            //                         .Skip((requestedPage - 1) * recordsInOnePage)
            //                         .Top(recordsInOnePage)
            //                         .GetAsync();

            //next page (CLIENT DRIVEN PAGINATION)
            var nextPageUrl = users.NextPageRequest.GetHttpRequestMessage().RequestUri.AbsoluteUri.ToString(); //sent by browser (received in previous response)

            var httpRqMethod_GET = new System.Net.Http.HttpMethod("GET");
            var nextPageHttpRq = new System.Net.Http.HttpRequestMessage(httpRqMethod_GET, nextPageUrl);
            var nextPageHttpRs = await graphClient.HttpProvider.SendAsync(nextPageHttpRq);
            var responseJSON = await nextPageHttpRs.Content.ReadAsStringAsync();

            //responseJSON - list of records for current page + next page request url

            //read profile
            var me = await graphClient.Me.Request().GetAsync();

            return View();
        }

        private Graph::GraphServiceClient GetGraphServiceClient(string[] scopes)
        {
            return GraphServiceClientFactory.GetAuthenticatedGraphClient(async () =>
            {
                string result = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
                return result;
            }, _webOptions.GraphApiUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
