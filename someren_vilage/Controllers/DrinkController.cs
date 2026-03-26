using Microsoft.AspNetCore.Mvc;
using someren_vilage.Models;
using someren_vilage.Repositorie.DrinkRepo;

namespace someren_vilage.Controllers
{
    public class DrinkController : Controller
    {
        private readonly IDrinkRepository _repo;

        public DrinkController(IDrinkRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index(string sort)
        {
            List<Drink> drinks = _repo.GetAll();

            drinks = sort switch
            {
                "name" => drinks.OrderBy(d => d.Name).ToList(),
                "name_desc" => drinks.OrderByDescending(d => d.Name).ToList(),
                "price" => drinks.OrderBy(d => d.Price).ToList(),
                "price_desc" => drinks.OrderByDescending(d => d.Price).ToList(),
                "stock" => drinks.OrderBy(d => d.Stock).ToList(),
                "stock_desc" => drinks.OrderByDescending(d => d.Stock).ToList(),
                _ => drinks.OrderBy(d => d.Name).ToList(),
            };

            ViewData["CurrentSort"] = sort;

            return View(drinks);
        }

        public IActionResult Create()
        {
            return View(new Drink());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Drink drink)
        {
            if (!ModelState.IsValid)
            {
                return View(drink);
            }

            _repo.Add(drink);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            Drink? drink = _repo.GetById(id);
            if (drink == null)
                return NotFound();

            return View(drink);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Drink drink)
        {
            if (!ModelState.IsValid)
                return View(drink);

            _repo.Update(drink);
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
