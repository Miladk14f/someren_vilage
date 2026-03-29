using Microsoft.AspNetCore.Mvc;
using someren_vilage.Models;
using someren_vilage.Repositorie.ActivityRepo;

namespace someren_vilage.Controllers
{
    public class ActivityController : Controller
    {
        private readonly IActivityRepository _repo;

        public ActivityController(IActivityRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult Index(string sort)
        {
            try
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
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<Activity>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                return View(new Activity());
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Activity activity)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(activity);
                }

                _repo.Add(activity);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(activity);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                Activity? activity = _repo.GetById(id);
                if (activity == null)
                    return NotFound();

                return View(activity);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Activity activity)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(activity);

                _repo.Update(activity);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
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
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

            }
        }
