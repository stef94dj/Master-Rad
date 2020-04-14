﻿using System.Threading.Tasks;
using MasterRad.DTO;
using MasterRad.DTO.RQ;
using MasterRad.Entities;
using MasterRad.Models;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using MasterRad.DTO.RS.TableRow;

namespace MasterRad.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SynthesisController : BaseController
    {
        private readonly ISynthesisRepository _synthesisRepo;
        private readonly IMsGraph _msGraph;

        public SynthesisController
        (
            ISynthesisRepository synthesisRepository,
            IMsGraph msGraph
        )
        {
            _synthesisRepo = synthesisRepository;
            _msGraph = msGraph;
        }

        [HttpGet, Route("get")]
        public async Task<ActionResult<IEnumerable<SynthesisTestDTO>>> GetTestsAsync()
        {
            var entities = _synthesisRepo.Get();

            #region Get_CreatedBy_Users_Details
            var createdByIds = entities.Select(e => e.CreatedBy);
            var createdByDetails = await _msGraph.GetStudentsByIds(createdByIds);
            #endregion

            #region Map_Result
            var res = entities.Select(entity =>
            {
                var createdByDetail = createdByDetails.Single(ud => ud.MicrosoftId == entity.CreatedBy);
                return new SynthesisTestDTO(entity, createdByDetail);
            });
            #endregion

            return Ok(res);
        }


        [HttpPost, Route("create/test")]
        public ActionResult<Result<bool>> CreateTest([FromBody] SynthesisCreateRQ body)
        {
            if (string.IsNullOrEmpty(body.Name))
                return Result<bool>.Fail("Name cannot be empty.");

            if (body.TaskId <= 0)
                return Result<bool>.Fail("A task must be selected.");

            var testExists = _synthesisRepo.TestExists(body.Name);
            if (testExists)
                return Result<bool>.Fail($"Test '{body.Name}' already exists.");

            var success = _synthesisRepo.Create(body, UserId);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("update/name")]
        public ActionResult<Result<bool>> UpdateTestName([FromBody] UpdateNameRQ body)
        {
            if (string.IsNullOrEmpty(body.Name))
                return Result<bool>.Fail("Name cannot be empty.");

            var testExists = _synthesisRepo.TestExists(body.Name);
            if (testExists)
                return Result<bool>.Fail($"Test '{body.Name}' already exists.");

            var success = _synthesisRepo.UpdateName(body, UserId);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("status/next")]
        public ActionResult<bool> GoToNextStatus([FromBody] UpdateDTO body)
            => _synthesisRepo.StatusNext(body, UserId);


        [HttpGet, Route("Solution/Format/{testId}")]
        public ActionResult<IEnumerable<string>> GetSolutionFormat([FromRoute] int testId)
            => Ok(_synthesisRepo.GetSolutionFormat(testId));

        [HttpPost, Route("delete/test")]
        public ActionResult<SynthesisTestEntity> DeleteTest([FromBody] DeleteDTO body)
        {
            _synthesisRepo.Delete(body);
            return Ok();
        }

        [HttpPost, Route("Submit/Answer")]
        public ActionResult<byte[]> SubmitAnswer([FromBody] AnswerSynthesisRQ body)
        {
            if (!_synthesisRepo.IsAssigned(UserId, body.TestId))
                return Unauthorized();

            return _synthesisRepo.SubmitAnswer(body.TestId, UserId, body.TimeStamp, body.SqlScript);
        }
    }
}
