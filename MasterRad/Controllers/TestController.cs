﻿using MasterRad.Attributes;
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
        [Authorize(Roles = UserRole.Student)]
        public IActionResult AnalysisExam(int testId, byte[] timeStamp)
        {
            var atsEntity = _analysisRepository.GetAssignment(UserId, testId);

            if (atsEntity == null)
                return Unauthorized();

            if (!atsEntity.TakenTest)
                _analysisRepository.MarkExamAsTaken(testId, UserId, timeStamp);

            var outputTablesDb = _adminConnectionConf.OutputTablesDbName;

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
