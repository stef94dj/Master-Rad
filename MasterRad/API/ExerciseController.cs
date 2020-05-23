using MasterRad.DTO.RQ;
using MasterRad.DTO.RS.Card;
using MasterRad.Models;
using MasterRad.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public ExerciseController
        (
            ITemplateRepository templateRepo
        )
        {
            _templateRepo = templateRepo;
        }

        [HttpGet, Route("get/templates")]
        public ActionResult<IEnumerable<TemplateDTO>> GetPublicTemplates()
        {
            var entities = _templateRepo.GetPublic();

            #region Map_Result
            var resData = entities.Select(entity =>
            {
                return new TemplateDTO(entity);
            });
            #endregion

            var res = entities.Select(entity =>
            {
                return new TemplateDTO(entity);
            });

            return Ok(res);
        }

        [HttpPost, Route("create/instance")]
        public ActionResult<Result<bool>> CreateInstance([FromBody] CreateExerciseInstanceRQ body)
        {
            return Result<bool>.Fail($"Instance '{body.Name}' already exists.");
        }
    }
}
