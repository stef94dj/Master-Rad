using MasterRad.DTO.RQ;
using MasterRad.DTO.RS.Card;
using MasterRad.Helpers;
using MasterRad.Models;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterRad.API
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRole.Student)]
    public class ExerciseController : BaseController
    {
        private readonly ITemplateRepository _templateRepo;
        private readonly IExerciseRepository _exerciseRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMicrosoftSQL _msSqlService;

        public ExerciseController
        (
            ITemplateRepository templateRepo,
            IExerciseRepository exerciseRepo,
            IUserRepository userRepo,
            IMicrosoftSQL msSqlService
        )
        {
            _templateRepo = templateRepo;
            _exerciseRepo = exerciseRepo;
            _userRepo = userRepo;
            _msSqlService = msSqlService;
        }

        [HttpGet, Route("get/templates")]
        public ActionResult<IEnumerable<TemplateDTO>> GetPublicTemplates()
        {
            var res = _templateRepo.GetPublic()
                                   .Select(entity => new TemplateDTO(entity));
            return Ok(res);
        }

        [HttpGet, Route("get/instances")]
        public ActionResult<IEnumerable<InstanceDTO>> GetExerciseInstances()
        {
            var res = _exerciseRepo.GetInstancesWithTemplates(UserId)
                                   .Select(entity => new InstanceDTO(entity));
            return Ok(res);
        }

        [HttpPost, Route("create/instance")]
        public ActionResult<Result<bool>> CreateInstance([FromBody] CreateExerciseInstanceRQ body)
        {
            #region Map_User
            var isUserMapped = _userRepo.IsMapped(UserId);
            if (!isUserMapped)
            {
                var sqlUsername = NameHelper.GenerateSqlUserName(UserId);
                var sqlPass = NameHelper.GenerateRandomSqlPassowrd();

                var mapSaveSuccess = _userRepo.CreateMapping(UserId, sqlUsername, sqlPass, UserId);
                if (!mapSaveSuccess)
                    return Result<bool>.Fail("Error mapping user.");
            }
            var userMapEntity = _userRepo.Get(UserId);
            #endregion

            #region Check_Name
            var exerciseExists = _exerciseRepo.NameExists(body.Name, UserId);
            if (exerciseExists)
                return Result<bool>.Fail($"Instance '{body.Name}' already exists.");
            #endregion

            #region Clone_DB
            var nameOnServer = NameHelper.ExerciseName();
            var templateEntity = _templateRepo.Get(body.TemplateId);
            var cloneSuccess = _msSqlService.CloneDatabase(templateEntity.NameOnServer, nameOnServer, false, true);
            if (!cloneSuccess)
                return Result<bool>.Fail("Failed to clone database.");
            #endregion

            #region Create_DB_User
            var createSuccess = _msSqlService.CreateDbUserContained(userMapEntity.SqlUsername, userMapEntity.SqlPassword, nameOnServer);
            if (!createSuccess)
                return Result<bool>.Fail("Failed to create database user.");

            var assignSuccess = _msSqlService.AssignCRUD(userMapEntity.SqlUsername, nameOnServer);
            if (!assignSuccess)
                return Result<bool>.Fail("Failed to assign database user permissions.");
            #endregion

            #region Save_Record
            var saveSuccess = _exerciseRepo.Create(body.TemplateId, UserId, body.Name, nameOnServer, UserId);
            if (!saveSuccess)
                return Result<bool>.Fail("Failed to save record.");
            #endregion

            return Result<bool>.Success(true);
        }

        [HttpPost, Route("delete/instance")]
        public ActionResult<Result<bool>> DeleteInstance([FromBody] DeleteEntityRQ body)
        {
            var instanceEntity = _exerciseRepo.Get(body.Id);
            if (instanceEntity.StudentId != UserId)
                return Unauthorized();

            var instanceDeleteSuccess = _msSqlService.DeleteDatabaseIfExists(instanceEntity.NameOnServer);
            if (!instanceDeleteSuccess)
                return Result<bool>.Fail("Failed to delete database instance.");

            var recordDeleteSuccess = _exerciseRepo.Delete(body.Id, body.TimeStamp);
            if(!recordDeleteSuccess)
                return Result<bool>.Fail("Failed to delete database record.");

            return Result<bool>.Success(true);
        }
    }
}
