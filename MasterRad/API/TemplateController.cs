﻿using MasterRad.DTOs;
using MasterRad.Entities;
using MasterRad.Helpers;
using MasterRad.Models;
using MasterRad.Models.Configuration;
using MasterRad.Models.DTOs;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp_OpenIDConnect_DotNet.Services;
using Graph = Microsoft.Graph;

namespace MasterRad.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : Controller
    {
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly ITemplateRepository _templateRepo;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly WebOptions _webOptions;
        //private readonly IConfiguration _config;

        public TemplateController
        (
            IMicrosoftSQL microsoftSQLService,
            ITemplateRepository templateRepo,
            ITokenAcquisition tokenAcquisition,
            IOptions<WebOptions> webOptions
        )
        {
            _microsoftSQLService = microsoftSQLService;
            _templateRepo = templateRepo;
            _tokenAcquisition = tokenAcquisition;
            _webOptions = webOptions.Value;
        }

        [HttpGet, Route("Get")]
        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserRead, Constants.ScopeUserReadBasicAll })]
        //AuthorizeForScopes: In case a token cannot be acquired, a challenge is attempted to re-sign-in the user, and have them consent to the requested scopes
        //Scenario: 
        //          1. user is on a page that calls AJAX endpoint (on a click of a button), the AJAX endpoint is a proxy to an MS Graph endpoint (e.g. User.ReadAll)
        //          2. server is restarted and session data (which is in memory) is lost
        //          3. user clicks the button but token cannot be resolved (no session data) - call to MS Graph throws exception
        //          4. AuthorizeForScopes OnException method handles the exceptions and sends challenge to browser context.Result = new ChallengeResult(properties); to try and re-sign-in the user
        //For MVC endpoints browser handles this correctly but not for AJAX endpoints: the link to re-signIn the user is sent to client but it get's blocked by CORS policy (see network)
        //double click on link in network opens the link successfully in new tab, allowing the user to be re-signedIn & ajax endpoint becomes available once again
        //the json in the new tab is actually the response from the ajax endpoint which got called again after user being re-signedIn
        //attempted to allow CORS in Starupt.cs with :
        //                  services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
        //                  {
        //                      builder.AllowAnyOrigin()
        //                             .AllowAnyMethod()
        //                             .AllowAnyHeader();
        //                  }));
        //but it didn't solve the issue

        public async Task<ActionResult<IEnumerable<TemplateEntity>>> GetTemplatesAsync() //=> _templateRepo.Get();
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

            return _templateRepo.Get();
        }

        private Graph::GraphServiceClient GetGraphServiceClient(string[] scopes)
        {
            return GraphServiceClientFactory.GetAuthenticatedGraphClient(async () =>
            {
                string result = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
                return result;
            }, _webOptions.GraphApiUrl) ;
        }

        [HttpPost, Route("Create")]
        public ActionResult<Result<bool>> CreateTemplate([FromBody] CreateTemplateRQ body)
        {
            if (string.IsNullOrEmpty(body.Name))
                return Result<bool>.Fail($"Name cannot be empty.");

            var templateExists = _templateRepo.TemplateExists(body.Name);
            if (templateExists)
                return Result<bool>.Fail($"Template '{body.Name}' already exists.");

            var newDbName = NameHelper.TemplateName();
            var alreadyRegistered = _templateRepo.DatabaseRegisteredAsTemplate(newDbName);
            if (alreadyRegistered)
                return Result<bool>.Fail($"Generated name is already used for another template. Please try again.");

            var existsOnSqlServer = _microsoftSQLService.DatabaseExists(newDbName);
            if (existsOnSqlServer)
                return Result<bool>.Fail($"Generated name is not unique. Please try again.");

            var dbCreateSuccess = _microsoftSQLService.CreateDatabase(newDbName, true);
            if (!dbCreateSuccess)
                return Result<bool>.Fail($"Failed to create databse '{newDbName}' on database server");

            var success = _templateRepo.Create(body.Name, newDbName);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("Update/Description")]
        public ActionResult<Result<bool>> UpdateDescription([FromBody] UpdateDescriptionRQ body)
        {
            var success = _templateRepo.UpdateDescription(body);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("Update/Name")]
        public ActionResult<Result<bool>> UpdateName([FromBody] UpdateNameRQ body)
        {
            if (string.IsNullOrEmpty(body.Name))
                return Result<bool>.Fail($"Template name cannot be empty.");

            var templateExists = _templateRepo.TemplateExists(body.Name);
            if (templateExists)
                return Result<bool>.Fail($"Template '{body.Name}' already exists in the system");

            var success = _templateRepo.UpdateName(body);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("Update/Model")]
        public ActionResult<Result<bool>> UpdateModel([FromBody] UpdateTemplateModelRQ body)
        {

            var templateEntity = _templateRepo.Get(body.Id);

            var scriptExeRes = _microsoftSQLService.ExecuteSQLAsAdmin(body.SqlScript, templateEntity.NameOnServer);

            if (scriptExeRes.Messages.Any())
                return Result<bool>.Fail(scriptExeRes.Messages);

            return Result<bool>.Success(true);
        }
    }
}
