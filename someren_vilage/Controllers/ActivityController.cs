using Microsoft.AspNetCore.Mvc;
using someren_vilage.Models;
using someren_vilage.Repositorie;

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

        public IActionResult Index()
        {
            try
            {
                var activities = _repo.GetAll();
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

        public IActionResult Delete(int id)
        {
            try
            {
                var activity = _repo.GetById(id);
                if (activity == null)
                    return NotFound();

                return View(activity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading activity for delete id {Id}", id);
                return StatusCode(500, "Unable to load activity for delete.");
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
