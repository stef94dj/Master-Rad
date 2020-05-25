using MasterRad.Models.ViewModels;
using MasterRad.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
                Name = entity.Name,
                DatabaseDescription = entity.Template.ModelDescription,
                NameOnServer = entity.NameOnServer
            };

            return View(vm);
        }

    }
}
