using MasterRad.Attributes;
using MasterRad.DTO;
using MasterRad.DTO.RQ;
using MasterRad.DTO.RS;
using MasterRad.DTO.RS.TableRow;
using MasterRad.Entities;
using MasterRad.Helpers;
using MasterRad.Models;
using MasterRad.Models.Configuration;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.API
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRole.Professor)]
    public class StudentController : BaseController
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ISynthesisRepository _synthesisRepository;
        private readonly IAnalysisRepository _analysisRepository;
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly SqlServerAdminConnection _adminConnectionConf;
        private readonly IMsGraph _msGraph;
        private readonly IUserRepository _userRepository;

        public StudentController
        (
            IStudentRepository studentRepository,
            ISynthesisRepository synthesisRepository,
            IAnalysisRepository analysisRepository,
            IMicrosoftSQL microsoftSQLService,
            IOptions<SqlServerAdminConnection> adminConnectionConf,
            IMsGraph msGraph,
            IUserRepository userRepository
        )
        {
            _studentRepository = studentRepository;
            _synthesisRepository = synthesisRepository;
            _analysisRepository = analysisRepository;
            _microsoftSQLService = microsoftSQLService;
            _adminConnectionConf = adminConnectionConf.Value;
            _msGraph = msGraph;
            _userRepository = userRepository;
        }

        [AjaxMsGraphProxy]
        [HttpPost, Route("azure/search")]
        public async Task<ActionResult<SearchStudentsRS>> SearchStudentsAsync([FromBody] SearchStudentsRQ body)
            => await _msGraph.SearchStudentsAsync(body);

        [AjaxMsGraphProxy]
        [HttpPost, Route("azure/page")]
        public async Task<ActionResult<SearchStudentsRS>> ListStudentsByPageAsync([FromBody] ListStudentsByPageRQ body)
            => await _msGraph.ListStudentsByPageAsync(body.PageUrl);

        [AjaxMsGraphProxy]
        [HttpGet, Route("get/assigned/{testType}/{testId}")]
        public async Task<ActionResult<IEnumerable<AssignedStudentDTO>>> GetStudentsAssignedToTestAsync([FromRoute]TestType testType, int testId)
        {
            IEnumerable<BaseTestStudentEntity> entities;
            switch (testType)
            {
                case TestType.Synthesis:
                    entities = _studentRepository.GetAssignedSynthesis(testId);
                    break;
                case TestType.Analysis:
                    entities = _studentRepository.GetAssignedAnalysis(testId);
                    break;
                default:
                    return StatusCode(500);
            }

            #region Get_Users_Details
            var userIds = entities.Select(e => e.StudentId);
            var userDetails = await _msGraph.GetStudentsByIds(userIds);
            #endregion

            #region Map_Result
            var res = entities.Select(entity =>
            {
                var userDetail = userDetails.Single(ud => ud.MicrosoftId == entity.StudentId);
                return new AssignedStudentDTO(entity, userDetail);
            });
            #endregion

            return Ok(res);
        }

        [HttpPost, Route("assign")]
        public ActionResult<Result<bool>> AssignStudentsToTest([FromBody] AssignStudentsRQ body)
        {
            var idsToMap = _userRepository.UnmappedIds(body.StudentIds);
            foreach (var id in idsToMap)
            {
                var sqlUsername = NameHelper.GenerateSqlUserName(id);
                var sqlPass = NameHelper.GenerateRandomSqlPassowrd();

                var mapSaveSuccess = _userRepository.CreateMapping(id, sqlUsername, sqlPass, UserId);
                if (!mapSaveSuccess)
                    Result<bool>.Fail($"Failed to save user mapping for {id}.");
            }

            return body.TestType switch
            {
                TestType.Synthesis => AssignStudentsToSynthesis(body),
                TestType.Analysis => AssignStudentsToAnalysis(body),
                _ => StatusCode(500),
            };
        }

        private ActionResult<Result<bool>> AssignStudentsToSynthesis(AssignStudentsRQ body)
        {
            var synthesisEntity = _synthesisRepository.GetWithTaskAndTemplate(body.TestId);

            if (synthesisEntity.Status >= TestStatus.Completed)
                return StatusCode(500);

            var task = synthesisEntity.Task;
            var template = task.Template;
            var userMapEntities = _userRepository.Get(body.StudentIds);
            foreach (var userMapEntity in userMapEntities)
            {
                oaisndoasnd
                  //CreateDbUserContained - reentrant
                  //CreateDbUserContained & AssignReadonly - maybe change to void & throw ex if fails?
                _microsoftSQLService.CreateDbUserContained(userMapEntity.SqlUsername, userMapEntity.SqlPassword, task.NameOnServer);
                _microsoftSQLService.AssignReadonly(userMapEntity.SqlUsername, task.NameOnServer);
                _microsoftSQLService.CreateDbUserContained(userMapEntity.SqlUsername, userMapEntity.SqlPassword, template.NameOnServer);
                _microsoftSQLService.AssignReadonly(userMapEntity.SqlUsername, template.NameOnServer);
            }

            var entitiesCnt = _studentRepository.AssignSynthesisTest(body.StudentIds, body.TestId, UserId);

            #region Return_Result
            if (entitiesCnt != body.StudentIds.Count())
                return Result<bool>.Fail("One or more students have not been assigned");
            else
                return Result<bool>.Success(true);
            #endregion
        }

        private ActionResult<Result<bool>> AssignStudentsToAnalysis(AssignStudentsRQ body)
        {
            if (_analysisRepository.Get(body.TestId).Status >= TestStatus.Completed)
                return StatusCode(500);

            var analysisEntity = _analysisRepository.GetWithTaskTemplateAndSolutionFormat(body.TestId);

            #region Get_Output_Format
            var columns = analysisEntity
                            .SynthesisTestStudent
                            .SynthesisTest
                            .Task
                            .SolutionColumns
                            .Select(c => new ColumnDTO(c.ColumnName, c.SqlType));
            #endregion

            var assignModels = NameHelper.AnalysisTestExam(body.StudentIds, analysisEntity.Id);

            #region Clone_Databases
            var analysisTemplateName = analysisEntity.SynthesisTestStudent.SynthesisTest.Task.Template.NameOnServer;
            var dbCloneSuccess = _microsoftSQLService.CloneDatabases(analysisTemplateName, assignModels.Select(am => am.Database), false);
            assignModels = assignModels.Where(x => dbCloneSuccess.Contains(x.Database));
            #endregion

            var outputTablesDbName = _adminConnectionConf.DbName;

            #region Create_Student_Output_Tables
            var studentTableCreateSuccess = _microsoftSQLService.CreateTables(assignModels.Select(x => new CreateTable()
            {
                DatabaseName = outputTablesDbName,
                TableName = x.StudentOutputTable,
                Columns = columns,
            }));
            assignModels = assignModels.Where(x => studentTableCreateSuccess.Contains(x.StudentOutputTable));
            #endregion

            #region Create_Teacher_Output_Tables
            var teacherTableCreateSuccess = _microsoftSQLService.CreateTables(assignModels.Select(x => new CreateTable()
            {
                DatabaseName = outputTablesDbName,
                TableName = x.TeacherOutputTable,
                Columns = columns,
            }));
            assignModels = assignModels.Where(x => teacherTableCreateSuccess.Contains(x.TeacherOutputTable));
            #endregion

            var analysisAssigned = _studentRepository.AssignAnalysisTest(assignModels, body.TestId, UserId);

            #region Return_Result
            if (analysisAssigned != new List<int>().Count()) //new List<int>()-> body.StudentIds
                return Result<bool>.Fail("One or more students have not been assigned");
            else
                return Result<bool>.Success(true);
            #endregion
        }

        [HttpPost, Route("remove/assigned")]
        public ActionResult<Result<bool>> RemoveStudentFromTest([FromBody] RemoveAssignedStudentRQ body)
        {
            var success = false;
            switch (body.TestType)
            {
                case TestType.Synthesis:
                    success = RemoveStudentFromSynthesis(body);
                    break;
                case TestType.Analysis:
                    success = RemoveStudentFromAnalysis(body);
                    break;
            }

            return success ? Result<bool>.Success(true) : Result<bool>.Fail("Operation failed");
        }

        private bool RemoveStudentFromSynthesis(RemoveAssignedStudentRQ model)
        {
            if (_synthesisRepository.Get(model.TestId).Status >= TestStatus.InProgress)
                return false;

            var assignment = _synthesisRepository.GetAssignment(model.StudentId, model.TestId);

            var dbDeleted = _microsoftSQLService.DeleteDatabaseIfExists(assignment.NameOnServer);
            if (!dbDeleted)
                return false;

            return _studentRepository.RemoveSynthesis(model.StudentId, model.TimeStamp, model.TestId);
        }

        private bool RemoveStudentFromAnalysis(RemoveAssignedStudentRQ model)
        {
            if (_analysisRepository.Get(model.TestId).Status >= TestStatus.InProgress)
                return false;

            var assignment = _analysisRepository.GetAssignment(model.StudentId, model.TestId);
            var outputTablesDbName = _adminConnectionConf.DbName;

            var dbDeleted = _microsoftSQLService.DeleteDatabaseIfExists(assignment.InputNameOnServer);
            var studOutTableDeleted = _microsoftSQLService.DeleteTableIfExists(assignment.StudentOutputNameOnServer, outputTablesDbName);
            var teacherOutTableDeleted = _microsoftSQLService.DeleteTableIfExists(assignment.TeacherOutputNameOnServer, outputTablesDbName);

            if (!dbDeleted || !studOutTableDeleted || !teacherOutTableDeleted)
                return false;

            return _studentRepository.RemoveAnalysis(model.StudentId, model.TimeStamp, model.TestId);
        }
    }
}
