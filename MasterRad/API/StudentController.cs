using MasterRad.DTO;
using MasterRad.DTO.RQ;
using MasterRad.DTO.RS;
using MasterRad.Entities;
using MasterRad.Helpers;
using MasterRad.Models;
using MasterRad.Models.Configuration;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ISynthesisRepository _synthesisRepository;
        private readonly IAnalysisRepository _analysisRepository;
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly SqlServerAdminConnection _adminConnectionConf;
        private readonly IMsGraph _msGraph;

        public StudentController
        (
            IStudentRepository studentRepository,
            ISynthesisRepository synthesisRepository,
            IAnalysisRepository analysisRepository,
            IMicrosoftSQL microsoftSQLService,
            IOptions<SqlServerAdminConnection> adminConnectionConf,
            IMsGraph msGraph
        )
        {
            _studentRepository = studentRepository;
            _synthesisRepository = synthesisRepository;
            _analysisRepository = analysisRepository;
            _microsoftSQLService = microsoftSQLService;
            _adminConnectionConf = adminConnectionConf.Value;
            _msGraph = msGraph;
        }

        [HttpPost, Route("azure/search")]
        public async Task<ActionResult<SearchStudentsRS>> SearchStudentsAsync([FromBody] SearchStudentsRQ body)
            => await _msGraph.SearchStudentsAsync(body);

        [HttpPost, Route("azure/page")]
        public async Task<ActionResult<SearchStudentsRS>> ListStudentsByPageAsync([FromBody] ListStudentsByPageRQ body)
            => await _msGraph.ListStudentsByPageAsync(body.PageUrl);

        [HttpGet, Route("get/assigned/{testType}/{testId}")]
        public ActionResult<IEnumerable<BaseTestStudentEntity>> GetStudentsAssignedToTest([FromRoute]TestType testType, int testId)
        {
            //get ms-graph ids of assigned students (set assignedStudentIds)
            switch (testType)
            {
                case TestType.Synthesis:
                    return Ok(_studentRepository.GetAssignedSynthesis(testId));
                case TestType.Analysis:
                    return Ok(_studentRepository.GetAssignedAnalysis(testId));
                default:
                    return StatusCode(500);
            }

            //get details on assigned students
            //var students = await _msGraph.GetStudentsByIds(assignedStudentIds).ToList());
        }

        [HttpPost, Route("assign")]
        public ActionResult<Result<bool>> AssignStudentsToTest([FromBody] AssignStudentsRQ body)
            => body.TestType switch
               {
                   TestType.Synthesis => AssignStudentsToSynthesis(body),
                   TestType.Analysis => AssignStudentsToAnalysis(body),
                   _ => StatusCode(500),
               };

        private ActionResult<Result<bool>> AssignStudentsToSynthesis(AssignStudentsRQ body)
        {
            var synthesisEntity = _synthesisRepository.GetWithTaskAndTemplate(body.TestId);

            if (synthesisEntity.Status >= TestStatus.Completed)
                return StatusCode(500);

            var synthesisExamDbNames = NameHelper.SynthesisTestExam(new List<int>(), synthesisEntity.Id); //new List<int>()-> body.StudentIds

            var synthesisTemplateName = synthesisEntity.Task.Template.NameOnServer;
            var synthesisCloneSuccess = _microsoftSQLService.CloneDatabases(synthesisTemplateName, synthesisExamDbNames.Select(snp => snp.Value), false);

            synthesisExamDbNames = synthesisExamDbNames.Where(x => synthesisCloneSuccess.Contains(x.Value));

            var synthesisAssigned = _studentRepository.AssignSynthesisTest(synthesisExamDbNames, body.TestId);
            if (synthesisAssigned != new List<int>().Count()) //new List<int>()-> body.StudentIds
                return Result<bool>.Fail("One or more students have not been assigned");
            else
                return Result<bool>.Success(true);
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

            var assignModels = NameHelper.AnalysisTestExam(new List<int>(), analysisEntity.Id); //new List<int>()-> body.StudentIds

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

            var analysisAssigned = _studentRepository.AssignAnalysisTest(assignModels, body.TestId);

            #region Return_Result
            if (analysisAssigned != new List<int>().Count()) //new List<int>()-> body.StudentIds
                return Result<bool>.Fail("One or more students have not been assigned");
            else
                return Result<bool>.Success(true);
            #endregion
        }

        [HttpPost, Route("remove/assigned")]
        public ActionResult<bool> RemoveStudentFromTest([FromBody] RemoveAssignedStudentRQ body)
        {
            switch (body.TestType)
            {
                case TestType.Synthesis:
                    if (!RemoveStudentFromSynthesis(body))
                        return StatusCode(500);
                    break;
                case TestType.Analysis:
                    if (!RemoveStudentFromAnalysis(body))
                        return StatusCode(500);
                    break;
                default:
                    return StatusCode(500);
            }

            return true;
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
