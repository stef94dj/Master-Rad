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
        private readonly IStudentRepository _studentRepo;
        private readonly ISynthesisRepository _synthesisRepo;
        private readonly IAnalysisRepository _analysisRepo;
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly SqlServerAdminConnection _adminConnectionConf;
        private readonly IMsGraph _msGraph;
        private readonly IUserRepository _userRepo;

        public StudentController
        (
            IStudentRepository studentRepo,
            ISynthesisRepository synthesisRepo,
            IAnalysisRepository analysisRepo,
            IMicrosoftSQL microsoftSQLService,
            IOptions<SqlServerAdminConnection> adminConnectionConf,
            IMsGraph msGraph,
            IUserRepository userRepo
        )
        {
            _studentRepo = studentRepo;
            _synthesisRepo = synthesisRepo;
            _analysisRepo = analysisRepo;
            _microsoftSQLService = microsoftSQLService;
            _adminConnectionConf = adminConnectionConf.Value;
            _msGraph = msGraph;
            _userRepo = userRepo;
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
                    entities = _studentRepo.GetAssignedSynthesis(testId);
                    break;
                case TestType.Analysis:
                    entities = _studentRepo.GetAssignedAnalysis(testId);
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
            #region Create_Missing_Azure_Sql_Mappings
            var mapSuccess = new List<Guid>();
            var idsToMap = _userRepo.UnmappedIds(body.StudentIds);
            foreach (var id in idsToMap)
            {
                var sqlUsername = NameHelper.GenerateSqlUserName(id);
                var sqlPass = NameHelper.GenerateRandomSqlPassowrd();

                var mapSaveSuccess = _userRepo.CreateMapping(id, sqlUsername, sqlPass, UserId);
                if (mapSaveSuccess)
                    mapSuccess.Add(id);
            }
            #endregion

            #region Assign_Synthesis_Or_Analysis
            var idsToAssign = mapSuccess.Union(body.StudentIds.Except(idsToMap));
            int successfullyAssigned;
            switch (body.TestType)
            {
                case TestType.Synthesis:
                    successfullyAssigned = AssignStudentsToSynthesis(idsToAssign, body.TestId);
                    break;
                case TestType.Analysis:
                    successfullyAssigned = AssignStudentsToAnalysis(idsToAssign, body.TestId);
                    break;
                default:
                    return Result<bool>.Fail("Invalid test type."); ;
            }
            #endregion

            #region Return_Response
            if (successfullyAssigned != body.StudentIds.Count())
                return Result<bool>.Fail("One or more students have not been assigned");
            else
                return Result<bool>.Success(true);
            #endregion
        }

        [NonAction]
        private int AssignStudentsToSynthesis(IEnumerable<Guid> studentIds, int testId)
        {
            if (_synthesisRepo.Get(testId).Status >= TestStatus.Completed)
                return 0;

            var synthesisEntity = _synthesisRepo.GetWithTaskAndTemplate(testId);
            var userMapEntities = _userRepo.Get(studentIds);

            var createContainedUserSuccessIds = new List<Guid>();
            #region Create_Contained_Users
            var task = synthesisEntity.Task;
            var template = task.Template;
            foreach (var userMapEntity in userMapEntities)
            {
                if (CreateContainedReadonly(userMapEntity.SqlUsername, userMapEntity.SqlPassword, template.NameOnServer))
                    createContainedUserSuccessIds.Add(userMapEntity.AzureId);
            }
            #endregion

            return _studentRepo.AssignSynthesisTest(createContainedUserSuccessIds, testId, UserId);
        }

        [NonAction]
        private bool CreateContainedReadonly(string sqlUsername, string sqlPassword, string dbName)
        {
            if (!_microsoftSQLService.UserExists(sqlUsername, dbName))
            {
                var createTaskSuccess = _microsoftSQLService.CreateDbUserContained(sqlUsername, sqlPassword, dbName);
                if (!createTaskSuccess)
                    return false;
            }

            if (!_microsoftSQLService.UserExists(sqlUsername, dbName))
                return false;

            return _microsoftSQLService.AssignReadonly(sqlUsername, dbName);
        }

        [NonAction]
        private int AssignStudentsToAnalysis(IEnumerable<Guid> studentIds, int testId)
        {
            if (_analysisRepo.Get(testId).Status >= TestStatus.Completed)
                return 0;

            var analysisEntity = _analysisRepo.GetWithTaskTemplateAndSolutionFormat(testId);

            #region Get_Output_Format
            var columns = analysisEntity
                            .SynthesisTestStudent
                            .SynthesisTest
                            .Task
                            .SolutionColumns
                            .Select(c => new ColumnDTO(c.ColumnName, c.SqlType));
            #endregion

            var assignModels = NameHelper.AnalysisTestExam(studentIds, analysisEntity.Id);

            #region Clone_Input_Databases
            var analysisTemplateName = analysisEntity.SynthesisTestStudent.SynthesisTest.Task.Template.NameOnServer;
            var dbCloneSuccess = _microsoftSQLService.CloneDatabases(analysisTemplateName, assignModels.Select(am => am.Database), false);
            assignModels = assignModels.Where(x => dbCloneSuccess.Contains(x.Database));
            #endregion

            var outputTablesDbName = _adminConnectionConf.OutputTablesDbName;

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

            #region Create_Contained_Users
            var userMapEntities = _userRepo.Get(assignModels.Select(am => am.StudentId)).ToList();
            var createContainedUserSuccessIds = new List<Guid>();
            foreach (var userMapEntity in userMapEntities)
            {
                var assignModel = assignModels.Single(am => am.StudentId == userMapEntity.AzureId);

                if (!CreateContainedCRUD(userMapEntity.SqlUsername, userMapEntity.SqlPassword, assignModel.Database))
                    continue;

                if (!CreateContainedCRUD(userMapEntity.SqlUsername, userMapEntity.SqlPassword, outputTablesDbName, assignModel.StudentOutputTable))
                    continue;

                if (!CreateContainedCRUD(userMapEntity.SqlUsername, userMapEntity.SqlPassword, outputTablesDbName, assignModel.TeacherOutputTable))
                    continue;

                createContainedUserSuccessIds.Add(userMapEntity.AzureId);
            }
            assignModels = assignModels.Where(x => createContainedUserSuccessIds.Contains(x.StudentId));
            #endregion

            return _studentRepo.AssignAnalysisTest(assignModels, testId, UserId);
        }

        [NonAction]
        private bool CreateContainedCRUD(string sqlUsername, string sqlPassword, string dbName, string tableName = null)
        {
            if (!_microsoftSQLService.UserExists(sqlUsername, dbName))
            {
                var createUserSuccess = _microsoftSQLService.CreateDbUserContained(sqlUsername, sqlPassword, dbName);
                if (!createUserSuccess)
                    return false;
            }

            if (!_microsoftSQLService.UserExists(sqlUsername, dbName))
                return false;

            if (string.IsNullOrEmpty(tableName))
                return _microsoftSQLService.AssignCRUD(sqlUsername, dbName);
            else
                return _microsoftSQLService.AssignCRUD(sqlUsername, dbName, tableName);
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
            if (_synthesisRepo.Get(model.TestId).Status >= TestStatus.InProgress)
                return false;

            return _studentRepo.RemoveSynthesis(model.StudentId, model.TimeStamp, model.TestId);
        }

        private bool RemoveStudentFromAnalysis(RemoveAssignedStudentRQ model)
        {
            if (_analysisRepo.Get(model.TestId).Status >= TestStatus.InProgress)
                return false;

            var assignment = _analysisRepo.GetAssignment(model.StudentId, model.TestId);
            var outputTablesDbName = _adminConnectionConf.DbName;

            var dbDeleted = _microsoftSQLService.DeleteDatabaseIfExists(assignment.InputNameOnServer);
            var studOutTableDeleted = _microsoftSQLService.DeleteTableIfExists(assignment.StudentOutputNameOnServer, outputTablesDbName);
            var teacherOutTableDeleted = _microsoftSQLService.DeleteTableIfExists(assignment.TeacherOutputNameOnServer, outputTablesDbName);

            if (!dbDeleted || !studOutTableDeleted || !teacherOutTableDeleted)
                return false;

            return _studentRepo.RemoveAnalysis(model.StudentId, model.TimeStamp, model.TestId);
        }
    }
}
