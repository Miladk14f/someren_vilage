using Microsoft.AspNetCore.Mvc;
using someren_vilage.Models;
using someren_vilage.Repositorie.StudentRepo;
using someren_vilage.Repositorie.RoomRepo;

namespace someren_vilage.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentRepository _repo;
        private readonly IRoomRepository _roomRepo;

        public StudentsController(IStudentRepository repo, IRoomRepository roomRepo)
        {
            _repo = repo;
            _roomRepo = roomRepo;
        }

        [HttpGet]
        public IActionResult Index(string sort)
        {
            try
            {
                List<Student> students = _repo.GetAll();

                students = sort switch
                {
                    "number_desc" => students.OrderByDescending(s => s.StudentNumber).ToList(),
                    "firstname" => students.OrderBy(s => s.FirstName).ToList(),
                    "firstname_desc" => students.OrderByDescending(s => s.FirstName).ToList(),
                    "lastname" => students.OrderBy(s => s.LastName).ToList(),
                    "lastname_desc" => students.OrderByDescending(s => s.LastName).ToList(),
                    "phone" => students.OrderBy(s => s.PhoneNumber).ToList(),
                    "phone_desc" => students.OrderByDescending(s => s.PhoneNumber).ToList(),
                    "class" => students.OrderBy(s => s.Class).ToList(),
                    "class_desc" => students.OrderByDescending(s => s.Class).ToList(),
                    "room" => students.OrderBy(s => s.RoomId).ToList(),
                    "room_desc" => students.OrderByDescending(s => s.RoomId).ToList(),
                    _ => students.OrderBy(s => s.StudentNumber).ToList(),
                };

                ViewData["CurrentSort"] = sort;

                return View(students);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<Student>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                ViewBag.Rooms = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_roomRepo.GetAll(), "RoomId", "RoomId");
                return View(new Student());
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student student)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Rooms = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_roomRepo.GetAll(), "RoomId", "RoomId", student.RoomId);
                    return View(student);
                }

                _repo.Add(student);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(student);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                Student? student = _repo.GetById(id);
                if (student == null)
                    return NotFound();

                ViewBag.Rooms = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_roomRepo.GetAll(), "RoomId", "RoomId", student.RoomId);
                return View(student);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student student)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Rooms = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_roomRepo.GetAll(), "RoomId", "RoomId", student.RoomId);
                    return View(student);
                }

                _repo.Update(student);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(student);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _repo.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}