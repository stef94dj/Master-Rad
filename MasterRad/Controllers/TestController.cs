using MasterRad.Attributes;
using MasterRad.Models.Configuration;
using MasterRad.Models.ViewModels;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System;

namespace MasterRad.Controllers
{
    public class TestController : BaseController
    {
        private readonly ISynthesisRepository _synthesisRepository;
        private readonly IAnalysisRepository _analysisRepository;
        private readonly SqlServerAdminConnection _adminConnectionConf;

        public TestController
        (
            ISynthesisRepository synthesisRepository,
            IAnalysisRepository analysisRepository,
            IOptions<SqlServerAdminConnection> adminConnectionConf
        )
        {
            _synthesisRepository = synthesisRepository;
            _analysisRepository = analysisRepository;
            _adminConnectionConf = adminConnectionConf.Value;
        }

        [Authorize(Roles = UserRole.Student)]
        public IActionResult SynthesisExam(int testId, byte[] timeStamp)
        {
            var stsEntity = _synthesisRepository.GetAssignmentWithTaskAndTemplate(UserId, testId);

            if (stsEntity == null)
                return Unauthorized();

            if (!stsEntity.TakenTest)
                _synthesisRepository.MarkExamAsTaken(testId, UserId, ref timeStamp);

            var taskEntity = stsEntity.SynthesisTest.Task;
            var vm = new SynthesisExamVM()
            {
                Title = stsEntity.SynthesisTest.Name,
                TestId = stsEntity.SynthesisTestId,
                StudentId = stsEntity.StudentId,
                TimeStamp = Convert.ToBase64String(timeStamp),
                NameOnServer = taskEntity.Template.NameOnServer,
                SqlScript = stsEntity.SqlScript ?? string.Empty,
                TaskDescription = taskEntity.Description,
                ModelDescription = taskEntity.Template.ModelDescription
            };

            return View(vm);
        }
        [Authorize(Roles = UserRole.Professor)]
        public IActionResult SynthesisExamReadonly(Guid studentId, int testId)
        {
            var stsEntity = _synthesisRepository.GetAssignmentWithTaskAndTemplate(studentId, testId);

            if (stsEntity == null || !stsEntity.TakenTest)
                throw new Exception("Test not found or taken");

            var taskEntity = stsEntity.SynthesisTest.Task;
            var vm = new SynthesisExamVM()
            {
                Title = stsEntity.SynthesisTest.Name,
                TestId = stsEntity.SynthesisTestId,
                StudentId = stsEntity.StudentId,
                NameOnServer = taskEntity.Template.NameOnServer,
                SqlScript = stsEntity.SqlScript ?? string.Empty,
                TaskDescription = taskEntity.Description,
                ModelDescription = taskEntity.Template.ModelDescription,
                ReadOnly = true
            };

            return View("SynthesisExam", vm);
        }

        [Authorize(Roles = UserRole.Student)]
        public IActionResult AnalysisExam(int testId, byte[] timeStamp)
        {
            var atsEntity = _analysisRepository.GetAssignmentWithTaskAndTemplate(UserId, testId);

            if (atsEntity == null)
                return Unauthorized();

            if (!atsEntity.TakenTest)
                _analysisRepository.MarkExamAsTaken(testId, UserId, timeStamp);

            var outputTablesDb = _adminConnectionConf.OutputTablesDbName;


            var sts = atsEntity.AnalysisTest.SynthesisTestStudent;
            var task = sts.SynthesisTest.Task;
            var vm = new AnalysisExamVM()
            {
                Title = $"{atsEntity.AnalysisTest.Name}",
                ModelDescription = task.Template.ModelDescription,
                TaskDescription = task.Description,
                SqlSolutionForEvaluation = sts.SqlScript,
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

        [Authorize(Roles = UserRole.Professor)]
        public IActionResult AnalysisExamReadonly(Guid studentId, int testId)
        {
            var atsEntity = _analysisRepository.GetAssignmentWithTaskAndTemplate(studentId, testId);

            if (atsEntity == null || !atsEntity.TakenTest)
                throw new Exception("Test not found or taken");


            var outputTablesDb = _adminConnectionConf.OutputTablesDbName;


            var sts = atsEntity.AnalysisTest.SynthesisTestStudent;
            var task = sts.SynthesisTest.Task;
            var vm = new AnalysisExamVM()
            {
                Title = $"Task '{atsEntity.AnalysisTest.Name}'",
                ModelDescription = task.Template.ModelDescription,
                TaskDescription = task.Description,
                SqlSolutionForEvaluation = sts.SqlScript,
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
                },
                Readonly = true
            };

            return View("~/Views/Test/AnalysisExam.cshtml", vm);
        }

        [Authorize(Roles = UserRole.Professor)]
        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserReadBasicAll })]
        [ImplicitAuthoriseForScopesTrigger(Scopes = new[] { Constants.ScopeUserReadBasicAll })]
        public IActionResult AssignStudents(int testId, TestType testType)
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

            var vm = new AssignStudentsVM
            {
                TestId = testId,
                TestType = testType,
                InitialPageSize = 10
            };
            return View(vm);
        }

        [Authorize(Roles = UserRole.Professor)]
        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserReadBasicAll })]
        [ImplicitAuthoriseForScopesTrigger(Scopes = new[] { Constants.ScopeUserReadBasicAll })]
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
