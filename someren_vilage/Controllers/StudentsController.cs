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

        public IActionResult Index()
        {
            List<Student> students = _repo.GetAll();
            return View(students);
        }

        public IActionResult Create()
        {
            ViewBag.Rooms = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_roomRepo.GetAll(), "RoomId", "RoomId");
            return View(new Student());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Rooms = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_roomRepo.GetAll(), "RoomId", "RoomId", student.RoomId);
                return View(student);
            }

            _repo.Add(student);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            Student? student = _repo.GetById(id);
            if (student == null)
                return NotFound();

            ViewBag.Rooms = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_roomRepo.GetAll(), "RoomId", "RoomId", student.RoomId);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student student)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Rooms = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_roomRepo.GetAll(), "RoomId", "RoomId", student.RoomId);
                return View(student);
            }

            _repo.Update(student);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}