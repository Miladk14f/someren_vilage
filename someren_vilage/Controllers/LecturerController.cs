using Microsoft.AspNetCore.Mvc;
using someren_vilage.Models;
using someren_vilage.Repositorie;

namespace someren_vilage.Controllers
{
    public class LecturerController : Controller
    {
        private readonly ILecturerRepository _lecturerRepository;

        private IRoomRepository _roomRepo;

        public LecturerController(ILecturerRepository lecturerRepository, IRoomRepository roomRepo)
        {
            _lecturerRepository = lecturerRepository;
            _roomRepo = roomRepo;
        }

        public IActionResult Index()
        {
            List<Lecturer> lecturers = _lecturerRepository.GetAll();
            return View(lecturers);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            Lecturer? lecturer = _lecturerRepository.GetById((int)id);
            return View(lecturer);
        }

        [HttpPost]
        public ActionResult Edit(Lecturer lecturer)
        {
            _lecturerRepository.Update(lecturer);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Lecturer? lecturer = _lecturerRepository.GetById((int)id);
            return View(lecturer);
        }

        [HttpPost]
        public ActionResult Delete(Lecturer lecturer)
        {
            try
            {
                _lecturerRepository.Delete(lecturer.LecturerId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(lecturer);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            //List<Room> rooms = _roomRepo.GetAll();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Lecturer lecturer)
        {
            try
            {
                _lecturerRepository.Add(lecturer);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                
                return View(lecturer);
            }
        }






    }
}
