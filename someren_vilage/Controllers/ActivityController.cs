using Microsoft.AspNetCore.Mvc;
using someren_vilage.Models;
using someren_vilage.Repositorie.ActivityRepo;
using someren_vilage.Repositorie.ParticipantRepo;
using someren_vilage.Repositorie.SupervisorRepo;

namespace someren_vilage.Controllers
{
    public class ActivityController : Controller
    {
        private readonly IActivityRepository _repo;
        private readonly IParticipantRepository _participantRepo;
        private readonly ISupervisorRepository _supervisorRepo;

        public ActivityController(IActivityRepository repo, IParticipantRepository participantRepo, ISupervisorRepository supervisorRepo)
        {
            _repo = repo;
            _participantRepo = participantRepo;
            _supervisorRepo = supervisorRepo;
        }

        public IActionResult Index(string sort)
        {
            List<Activity> activities = _repo.GetAll();

            activities = sort switch
            {
                "name" => activities.OrderBy(a => a.Name).ToList(),
                "name_desc" => activities.OrderByDescending(a => a.Name).ToList(),
                "day" => activities.OrderBy(a => a.Day).ToList(),
                "day_desc" => activities.OrderByDescending(a => a.Day).ToList(),
                "time" => activities.OrderBy(a => a.TimeOfDay).ToList(),
                "time_desc" => activities.OrderByDescending(a => a.TimeOfDay).ToList(),
                _ => activities.OrderBy(a => a.Name).ToList(),
            };

            ViewData["CurrentSort"] = sort;

            return View(activities);
        }

        public IActionResult Create()
        {
            return View(new Activity());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Activity activity)
        {
            if (!ModelState.IsValid)
            {
                return View(activity);
            }

            _repo.Add(activity);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            Activity? activity = _repo.GetById(id);
            if (activity == null)
                return NotFound();

            return View(activity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Activity activity)
        {
            if (!ModelState.IsValid)
                return View(activity);

            _repo.Update(activity);
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

        public IActionResult Participent(int id)
        {
            Activity? activity = _repo.GetById(id);
            if (activity == null) return NotFound();

            ViewModels.ActivityParticipantsViewModel model = new ViewModels.ActivityParticipantsViewModel
            {
                Activity = activity,
                Participants = _participantRepo.GetParticipants(id),
                Lecturers = _supervisorRepo.GetSupervisors(id),
                AllStudents = _participantRepo.GetAllStudents(),
                AllLecturers = _supervisorRepo.GetAllLecturers()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddParticipant(int activityId, int studentNumber)
        {
            _participantRepo.AddParticipant(activityId, studentNumber);
            return RedirectToAction(nameof(Participent), new { id = activityId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveParticipant(int activityId, int studentNumber)
        {
            _participantRepo.RemoveParticipant(activityId, studentNumber);
            return RedirectToAction(nameof(Participent), new { id = activityId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddLecturer(int activityId, int lecturerId)
        {
            _supervisorRepo.AddSupervisor(activityId, lecturerId);
            return RedirectToAction(nameof(Participent), new { id = activityId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveLecturer(int activityId, int lecturerId)
        {
            _supervisorRepo.RemoveSupervisor(activityId, lecturerId);
            return RedirectToAction(nameof(Participent), new { id = activityId });
        }
    }
}
