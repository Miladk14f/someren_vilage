using Microsoft.AspNetCore.Mvc;
using someren_vilage.Models;
using someren_vilage.Repositorie;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace someren_vilage.Controllers
{
    public class RoomController : Controller
    {
        private readonly someren_vilage.Repositorie.IRoomRepository _repo;
        private readonly ILogger<RoomController> _logger;

        public RoomController(someren_vilage.Repositorie.IRoomRepository repo, ILogger<RoomController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var rooms = _repo.GetAll();
                return View(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading room list");
                return StatusCode(500, "Unable to load rooms.");
            }
        }

        public IActionResult Create()
        {
            try
            {
                return View(new Room());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error displaying create room page");
                return StatusCode(500, "Unable to display create page.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Room room)
        {
            if (!ModelState.IsValid)
            {
                return View(room);
            }
            try
            {
                _repo.Add(room);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating room");
                ModelState.AddModelError("", "Unable to create room. " + ex.Message);
                return View(room);
            }
        }

        public IActionResult Edit(int id)
        {
            try
            {
                var room = _repo.GetById(id);
                if (room == null)
                    return NotFound();

                _logger.LogDebug("Edit GET room {Id}: Floor={Floor}, Type={Type}, Capacity={Capacity}",
                    room.RoomId, room.Floor, room.RoomType, room.Capacity);

                return View(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading room for edit id {Id}", id);
                return StatusCode(500, "Unable to load room for edit.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Room room)
        {
            if (!ModelState.IsValid)
                return View(room);

            if (room.RoomId <= 0)
            {
                ModelState.AddModelError("", "Invalid room id.");
                return View(room);
            }

            try
            {
                _repo.Update(room);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Update failed for room id {RoomId}", room.RoomId);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating room id {RoomId}", room.RoomId);
                ModelState.AddModelError("", "Unable to update room. " + ex.Message);
                return View(room);
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                var room = _repo.GetById(id);
                if (room == null)
                    return NotFound();

                return View(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading room for delete id {Id}", id);
                return StatusCode(500, "Unable to load room for delete.");
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
                _logger.LogError(ex, "Error deleting room id {Id}", id);
                return StatusCode(500, "Unable to delete room.");
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
