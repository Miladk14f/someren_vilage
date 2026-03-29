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

        [HttpGet]
        public IActionResult Index(string sort)
        {
            try
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
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<Drink>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                return View(new Drink());
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Drink drink)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(drink);
                }

                _repo.Add(drink);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(drink);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                Drink? drink = _repo.GetById(id);
                if (drink == null)
                    return NotFound();

                return View(drink);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Drink drink)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(drink);

                _repo.Update(drink);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(drink);
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
