using Microsoft.AspNetCore.Mvc;
using someren_vilage.Models;
using someren_vilage.Repositorie.LecturerRepo;
using someren_vilage.Repositorie.RoomRepo;

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

        [HttpGet]
        public IActionResult Index(string sort)
        {
            try
            {
                List<Lecturer> lecturers = _lecturerRepository.GetAll();

                lecturers = sort switch
                {
                    "firstname" => lecturers.OrderBy(l => l.FirstName).ToList(),
                    "firstname_desc" => lecturers.OrderByDescending(l => l.FirstName).ToList(),
                    "lastname_desc" => lecturers.OrderByDescending(l => l.LastName).ToList(),
                    "phone" => lecturers.OrderBy(l => l.PhoneNumber).ToList(),
                    "phone_desc" => lecturers.OrderByDescending(l => l.PhoneNumber).ToList(),
                    "age" => lecturers.OrderBy(l => l.Age).ToList(),
                    "age_desc" => lecturers.OrderByDescending(l => l.Age).ToList(),
                    "room" => lecturers.OrderBy(l => l.RoomId).ToList(),
                    "room_desc" => lecturers.OrderByDescending(l => l.RoomId).ToList(),
                    _ => lecturers.OrderBy(l => l.LastName).ToList(),
                };

                ViewData["CurrentSort"] = sort;

                return View(lecturers);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<Lecturer>());
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            try
            {
                Lecturer? lecturer = _lecturerRepository.GetById((int)id);
                ViewBag.Rooms = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_roomRepo.GetAll(), "RoomId", "RoomId", lecturer?.RoomId);
                return View(lecturer);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Edit(Lecturer lecturer)
        {
            try
            {
                _lecturerRepository.Update(lecturer);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Rooms = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_roomRepo.GetAll(), "RoomId", "RoomId", lecturer.RoomId);
                TempData["Error"] = ex.Message;
                return View(lecturer);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _lecturerRepository.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                ViewBag.Rooms = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_roomRepo.GetAll(), "RoomId", "RoomId");
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
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
                ViewBag.Rooms = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_roomRepo.GetAll(), "RoomId", "RoomId", lecturer.RoomId);
                TempData["Error"] = ex.Message;
                return View(lecturer);
            }
        }






    }
}
