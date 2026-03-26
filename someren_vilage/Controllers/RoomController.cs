using Microsoft.AspNetCore.Mvc;
using someren_vilage.Models;
using someren_vilage.Repositorie.RoomRepo;

namespace someren_vilage.Controllers
{
    public class RoomController : Controller
    {
        private readonly IRoomRepository _repo;

        public RoomController(IRoomRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index(string sort)
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

        public IActionResult Create()
        {
            return View(new Room());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Room room)
        {
            if (!ModelState.IsValid)
            {
                return View(room);
            }

            _repo.Add(room);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            Room? room = _repo.GetById(id);
            if (room == null)
                return NotFound();

            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Room room)
        {
            if (!ModelState.IsValid)
                return View(room);

            _repo.Update(room);
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
