using MasterRad.Models.ViewModels;
using MasterRad.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace MasterRad.Controllers
{
    [Authorize(Roles = UserRole.Student)]
    public class ExerciseController : BaseController
    {
        private readonly IExerciseRepository _exerciseRepository;

        public ExerciseController(IExerciseRepository exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }

        public IActionResult Instance(int exerciseId)
        {
            var entity = _exerciseRepository.GetWithTemplate(exerciseId);
            if (UserId != entity.StudentId)
                return Unauthorized();

            var vm = new ExerciseInstanceVM()
            {
                InstanceId = entity.Id,
                InstanceTimeStamp = Convert.ToBase64String(entity.TimeStamp),
                Name = entity.Name,
                DatabaseDescription = entity.Template.ModelDescription,
                NameOnServer = entity.NameOnServer,
                SqlScript = entity.SqlScript
            };

            return View(vm);
        }

    }
}
