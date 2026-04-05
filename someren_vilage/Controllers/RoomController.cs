using Microsoft.AspNetCore.Mvc;
using someren_vilage.Models;
using someren_vilage.Repositorie.LecturerRepo;
using someren_vilage.Repositorie.RoomRepo;
using someren_vilage.Repositorie.StudentRepo;

namespace someren_vilage.Controllers
{
    public class RoomController : Controller
    {
        private readonly IRoomRepository _repo;
        private readonly IStudentRepository _studentRepo;
        private readonly ILecturerRepository _lecturerRepo;

        public RoomController(IRoomRepository repo, IStudentRepository studentRepo, ILecturerRepository lecturerRepo)
        {
            _repo = repo;
            _studentRepo = studentRepo;
            _lecturerRepo = lecturerRepo;
        }

        [HttpGet]
        public IActionResult Index(string sort)
        {
            try
            {
                List<Room> rooms = _repo.GetAll();

                rooms = sort switch
                {
                    "floor" => rooms.OrderBy(r => r.Floor).ToList(),
                    "floor_desc" => rooms.OrderByDescending(r => r.Floor).ToList(),
                    "type" => rooms.OrderBy(r => r.RoomType).ToList(),
                    "type_desc" => rooms.OrderByDescending(r => r.RoomType).ToList(),
                    "capacity" => rooms.OrderBy(r => r.Capacity).ToList(),
                    "capacity_desc" => rooms.OrderByDescending(r => r.Capacity).ToList(),
                    "building" => rooms.OrderBy(r => r.BuildingName).ToList(),
                    "building_desc" => rooms.OrderByDescending(r => r.BuildingName).ToList(),
                    "id_desc" => rooms.OrderByDescending(r => r.RoomId).ToList(),
                    _ => rooms.OrderBy(r => r.RoomId).ToList(),
                };

                ViewData["CurrentSort"] = sort;

                return View(rooms);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<Room>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                return View(new Room());
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Room room)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(room);
                }

                _repo.Add(room);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(room);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                Room? room = _repo.GetById(id);
                if (room == null)
                    return NotFound();

                return View(room);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Room room)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(room);

                _repo.Update(room);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(room);
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

        [HttpGet]
        public IActionResult ManageStudents(int id)
        {
            try
            {
                Room? room = _repo.GetById(id);
                if (room == null)
                    return NotFound();

                List<Student> allStudents = _studentRepo.GetAll();
                List<Lecturer> allLecturers = _lecturerRepo.GetAll();

                ViewModels.RoomStudentsViewModel model = new ViewModels.RoomStudentsViewModel
                {
                    Room = room,
                    AssignedStudents = allStudents.Where(s => s.RoomId == id).ToList(),
                    UnassignedStudents = allStudents.Where(s => s.RoomId == null || s.RoomId == 0).ToList(),
                    AssignedLecturers = allLecturers.Where(l => l.RoomId == id).ToList(),
                    UnassignedLecturers = allLecturers.Where(l => l.RoomId == null || l.RoomId == 0).ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStudent(int roomId, int studentNumber)
        {
            try
            {
                if (studentNumber == 0)
                {
                    TempData["Error"] = "Please select a student before adding.";
                    return RedirectToAction(nameof(ManageStudents), new { id = roomId });
                }

                Student? student = _studentRepo.GetById(studentNumber);
                if (student == null)
                    return NotFound();

                student.RoomId = roomId;
                _studentRepo.Update(student);
                return RedirectToAction(nameof(ManageStudents), new { id = roomId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(ManageStudents), new { id = roomId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveStudent(int roomId, int studentNumber)
        {
            try
            {
                Student? student = _studentRepo.GetById(studentNumber);
                if (student == null)
                    return NotFound();

                student.RoomId = null;
                _studentRepo.Update(student);
                return RedirectToAction(nameof(ManageStudents), new { id = roomId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(ManageStudents), new { id = roomId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddLecturer(int roomId, int lecturerId)
        {
            try
            {
                if (lecturerId == 0)
                {
                    TempData["Error"] = "Please select a lecturer before adding.";
                    return RedirectToAction(nameof(ManageStudents), new { id = roomId });
                }

                Lecturer? lecturer = _lecturerRepo.GetById(lecturerId);
                if (lecturer == null)
                    return NotFound();

                lecturer.RoomId = roomId;
                _lecturerRepo.Update(lecturer);
                return RedirectToAction(nameof(ManageStudents), new { id = roomId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(ManageStudents), new { id = roomId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveLecturer(int roomId, int lecturerId)
        {
            try
            {
                Lecturer? lecturer = _lecturerRepo.GetById(lecturerId);
                if (lecturer == null)
                    return NotFound();

                lecturer.RoomId = null;
                _lecturerRepo.Update(lecturer);
                return RedirectToAction(nameof(ManageStudents), new { id = roomId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(ManageStudents), new { id = roomId });
            }
        }
    }
}
