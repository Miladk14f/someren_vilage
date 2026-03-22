using Microsoft.AspNetCore.Mvc;
using someren_vilage.Models;
using someren_vilage.Repositorie;
using System.Linq;

namespace someren_vilage.Controllers
{
    public class ActivityController : Controller
    {
        private readonly IActivityRepository _repo;
        private readonly ILogger<ActivityController> _logger;

        public ActivityController(IActivityRepository repo, ILogger<ActivityController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public IActionResult Participent(int id)
        {
            try
            {
                var activity = _repo.GetById(id);
                if (activity == null) return NotFound();

                var model = new ViewModels.ActivityParticipantsViewModel
                {
                    Activity = activity,
                    Participants = _repo.GetParticipants(id),
                    Lecturers = _repo.GetLecturers(id),
                    AllStudents = _repo.GetAllStudents(),
                    AllLecturers = _repo.GetAllLecturers()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading participants for activity {Id}", id);
                return StatusCode(500, "Unable to load participants.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddParticipant(int activityId, int studentNumber)
        {
            try
            {
                _repo.AddParticipant(activityId, studentNumber);
                TempData["Success"] = "Participant added.";
                return RedirectToAction(nameof(Participent), new { id = activityId });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule prevented adding participant {Student} to activity {Activity}", studentNumber, activityId);
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Participent), new { id = activityId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding participant {Student} to activity {Activity}", studentNumber, activityId);
                TempData["Error"] = "Unable to add participant.";
                return RedirectToAction(nameof(Participent), new { id = activityId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveParticipant(int activityId, int studentNumber)
        {
            try
            {
                _repo.RemoveParticipant(activityId, studentNumber);
                TempData["Success"] = "Participant removed.";
                return RedirectToAction(nameof(Participent), new { id = activityId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing participant {Student} from activity {Activity}", studentNumber, activityId);
                TempData["Error"] = "Unable to remove participant.";
                return RedirectToAction(nameof(Participent), new { id = activityId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddLecturer(int activityId, int lecturerId)
        {
            try
            {
                _repo.AddLecturer(activityId, lecturerId);
                TempData["Success"] = "Lecturer added.";
                return RedirectToAction(nameof(Participent), new { id = activityId });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule prevented adding lecturer {Lecturer} to activity {Activity}", lecturerId, activityId);
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Participent), new { id = activityId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding lecturer {Lecturer} to activity {Activity}", lecturerId, activityId);
                TempData["Error"] = "Unable to add lecturer.";
                return RedirectToAction(nameof(Participent), new { id = activityId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveLecturer(int activityId, int lecturerId)
        {
            try
            {
                _repo.RemoveLecturer(activityId, lecturerId);
                TempData["Success"] = "Lecturer removed.";
                return RedirectToAction(nameof(Participent), new { id = activityId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing lecturer {Lecturer} from activity {Activity}", lecturerId, activityId);
                TempData["Error"] = "Unable to remove lecturer.";
                return RedirectToAction(nameof(Participent), new { id = activityId });
            }
        }

        public IActionResult Index(string sort)
        {
            try
            {
                var activities = _repo.GetAll() ?? Enumerable.Empty<Activity>();
                activities = sort switch
                {
                    "name" => activities.OrderBy(a => a.Name),
                    "name_desc" => activities.OrderByDescending(a => a.Name),
                    "day" => activities.OrderBy(a => a.Day),
                    "day_desc" => activities.OrderByDescending(a => a.Day),
                    "time" => activities.OrderBy(a => a.TimeOfDay),
                    "time_desc" => activities.OrderByDescending(a => a.TimeOfDay),
                    _ => activities.OrderBy(a => a.Name),
                };

                ViewData["CurrentSort"] = sort;

                return View(activities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading activity list");
                return StatusCode(500, "Unable to load activities.");
            }
        }

        public IActionResult Create()
        {
            try
            {
                return View(new Activity());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error displaying create activity page");
                return StatusCode(500, "Unable to display create page.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Activity activity)
        {
            _logger.LogDebug("Create POST called. Activity model: {@Activity}", activity);

            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    var key = entry.Key;
                    foreach (var error in entry.Value.Errors)
                    {
                        var msg = error.ErrorMessage;
                        if (string.IsNullOrEmpty(msg) && error.Exception != null)
                            msg = error.Exception.Message;
                        _logger.LogWarning("ModelState error for '{Key}': {Error}", key, msg);
                    }
                }

                return View(activity);
            }

            try
            {
                _logger.LogDebug("ModelState valid. Calling repository Add.");
                _repo.Add(activity);
                _logger.LogInformation("Activity created with id {ActivityId}", activity.ActivityId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating activity");
                ModelState.AddModelError("", "Unable to create activity. " + ex.Message);
                return View(activity);
            }
        }

        public IActionResult Edit(int id)
        {
            try
            {
                var activity = _repo.GetById(id);
                if (activity == null)
                    return NotFound();

                _logger.LogDebug("Edit GET activity {Id}: Name={Name}, Day={Day}, Time={Time}",
                    activity.ActivityId, activity.Name, activity.Day, activity.TimeOfDay);

                return View(activity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading activity for edit id {Id}", id);
                return StatusCode(500, "Unable to load activity for edit.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Activity activity)
        {
            if (!ModelState.IsValid)
                return View(activity);

            if (activity.ActivityId <= 0)
            {
                ModelState.AddModelError("", "Invalid activity id.");
                return View(activity);
            }

            try
            {
                _repo.Update(activity);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Update failed for activity id {ActivityId}", activity.ActivityId);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating activity id {ActivityId}", activity.ActivityId);
                ModelState.AddModelError("", "Unable to update activity. " + ex.Message);
                return View(activity);
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
            catch (InvalidOperationException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx && sqlEx.Number == 547)
            {
                _logger.LogWarning(ex, "Cannot delete activity id {Id} due to foreign key constraint", id);
                TempData["DeleteError"] = $"Cannot delete activity {id}: this activity is currently assigned to one or more students or lecturers. Please remove them from this activity first.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting activity id {Id}", id);
                return StatusCode(500, "Unable to delete activity.");
            }
        }

        [HttpPost]  
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
