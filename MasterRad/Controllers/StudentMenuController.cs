using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterRad.Controllers
{
    [Authorize(Roles = UserRole.Student)]
    public class StudentMenuController : BaseController
    {
        public IActionResult Exercises()
            => View();

        public IActionResult Tests()
            => View();

        public IActionResult StartTest()
            => View("~/Views/StudentMenu/Exercises.cshtml");

    }
}
