using MasterRad.DTO;
using MasterRad.DTO.RQ;
using MasterRad.Models;
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
        private readonly ISynthesisRepository _synthesisRepository;
        private readonly IAnalysisRepository _analysisRepository;
        private readonly SqlServerAdminConnection _adminConnectionConf;
        private readonly IMsGraph _msGraph;

        public TestController
        (
            ISynthesisRepository synthesisRepository,
            IAnalysisRepository analysisRepository,
            IOptions<SqlServerAdminConnection> adminConnectionConf,
            IMsGraph msGraph
        )
        {
            _synthesisRepository = synthesisRepository;
            _analysisRepository = analysisRepository;
            _adminConnectionConf = adminConnectionConf.Value;
            _msGraph = msGraph;
        }

        public IActionResult SynthesisExam(int testId, byte[] timeStamp)
        {
            var stsEntity = _synthesisRepository.GetAssignmentWithTaskAndTemplate(Guid.Empty, testId);

            if (stsEntity == null)
                return Unauthorized();

            if (!stsEntity.TakenTest)
                _synthesisRepository.MarkExamAsTaken(testId, Guid.Empty, timeStamp);

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
            var atsEntity = _analysisRepository.GetAssignment(Guid.Empty, testId);

            if (atsEntity == null)
                return Unauthorized();

            if (!atsEntity.TakenTest)
                _analysisRepository.MarkExamAsTaken(testId, Guid.Empty, timeStamp);

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

            var initialPageSize = 15;
            var ssrq = new SearchStudentsRQ(initialPageSize);

            var students = await _msGraph.SearchStudentsAsync(ssrq);

            var vm = new AssignStudentsVM
            {
                TestId = testId,
                TestType = testType,
                StudentSearchRes = students,
                InitialPageSize = initialPageSize
            };
            return View(vm);
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
