using MasterRad.Models.Configuration;
using MasterRad.Models.ViewModels;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System;
using System.Threading.Tasks;
using WebApp_OpenIDConnect_DotNet.Services;
using Graph = Microsoft.Graph;

namespace MasterRad.Controllers
{
    public class TestController : Controller
    {
        private readonly IUser _userService;
        private readonly ISynthesisRepository _synthesisRepository;
        private readonly IAnalysisRepository _analysisRepository;
        private readonly SqlServerAdminConnection _adminConnectionConf;
        readonly ITokenAcquisition _tokenAcquisition;
        readonly WebOptions _webOptions;

        public TestController
        (
            IUser userService,
            ISynthesisRepository synthesisRepository,
            IAnalysisRepository analysisRepository,
            IOptions<SqlServerAdminConnection> adminConnectionConf,
            ITokenAcquisition tokenAcquisition,
            IOptions<WebOptions> webOptions
        )
        {
            _userService = userService;
            _synthesisRepository = synthesisRepository;
            _analysisRepository = analysisRepository;
            _adminConnectionConf = adminConnectionConf.Value;
            _tokenAcquisition = tokenAcquisition;
            _webOptions = webOptions.Value;
        }

        public IActionResult SynthesisExam(int testId, byte[] timeStamp)
        {
            var stsEntity = _synthesisRepository.GetAssignmentWithTaskAndTemplate(_userService.UserId, testId);

            if (stsEntity == null)
                return Unauthorized();

            if (!stsEntity.TakenTest)
                _synthesisRepository.MarkExamAsTaken(testId, _userService.UserId, timeStamp);

            var vm = new SynthesisExamVM()
            {
                TestId = stsEntity.SynthesisTestId,
                StudentId = stsEntity.StudentId,
                TimeStamp = Convert.ToBase64String(stsEntity.TimeStamp),
                NameOnServer = stsEntity.NameOnServer,
                SqlScript = stsEntity.SqlScript ?? string.Empty,
                TaskDescription = stsEntity.SynthesisTest.Task.Description,
                ModelDescription = stsEntity.SynthesisTest.Task.Template.ModelDescription
            };

            return View(vm);
        }
        public IActionResult AnalysisExam(int testId, byte[] timeStamp)
        {
            var atsEntity = _analysisRepository.GetAssignment(_userService.UserId, testId);

            if (atsEntity == null)
                return Unauthorized();

            if (!atsEntity.TakenTest)
                _analysisRepository.MarkExamAsTaken(testId, _userService.UserId, timeStamp);

            var outputTablesDb = _adminConnectionConf.DbName;

            var vm = new AnalysisExamVM()
            {
                Title = $"Task '{atsEntity.AnalysisTest.Name}'",
                FailingInputVM = new ModifyDatabasePartialVM()
                {
                    NameOnServer = atsEntity.InputNameOnServer
                },
                StudentOutputVM = new ModifyTablePartialVM()
                {
                    NameOnServer = outputTablesDb,
                    TableName = atsEntity.StudentOutputNameOnServer
                },
                CorrectOutputVM = new ModifyTablePartialVM()
                {
                    NameOnServer = outputTablesDb,
                    TableName = atsEntity.TeacherOutputNameOnServer
                }
            };

            return View("~/Views/Test/AnalysisExam.cshtml", vm);
        }

        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserReadBasicAll })]
        public async Task<IActionResult> AssignStudentsAsync(int testId, TestType testType)
        {
            switch (testType)
            {
                case TestType.Synthesis:
                    if (_synthesisRepository.Get(testId).Status >= TestStatus.Completed)
                        return StatusCode(500);
                    break;
                case TestType.Analysis:
                    if (_analysisRepository.Get(testId).Status >= TestStatus.Completed)
                        return StatusCode(500);
                    break;
                default:
                    return StatusCode(500);
            }

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

            var vm = new AssignStudentsVM
            {
                TestId = testId,
                TestType = testType
            };
            return View(vm);
        }

        private Graph::GraphServiceClient GetGraphServiceClient(string[] scopes)
        {
            return GraphServiceClientFactory.GetAuthenticatedGraphClient(async () =>
            {
                string result = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
                return result;
            }, _webOptions.GraphApiUrl);
        }

        public IActionResult Results(int testId, TestType testType)
        {
            if (testType != TestType.Synthesis && testType != TestType.Analysis)
                throw new Exception($"Invalid test type '{testType}'");

            var vm = new TestResultsVM
            {
                TestId = testId,
                TestType = testType
            };
            return View(vm);
        }
    }
}
