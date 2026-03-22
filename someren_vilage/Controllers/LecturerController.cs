using Microsoft.AspNetCore.Mvc;
using someren_vilage.Models;
using someren_vilage.Repositorie;

namespace someren_vilage.Controllers
{
    public class LecturerController : Controller
    {
        private readonly ILecturerRepository _lecturerRepository;
        public LecturerController(ILecturerRepository lecturerRepository)
        {
            _lecturerRepository = lecturerRepository;
        }

        public IActionResult Index()
        {
            List<Lecturer> lecturers = _lecturerRepository.GetAll();
            return View(lecturers);
        }
    }
}
