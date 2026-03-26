using Microsoft.AspNetCore.Mvc;
using someren_vilage.Models;
using someren_vilage.Repositorie.DrinkRepo;
using someren_vilage.Repositorie.OrderRepo;

namespace someren_vilage.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IDrinkRepository _drinkRepo;

        public OrderController(IOrderRepository orderRepo, IDrinkRepository drinkRepo)
        {
            _orderRepo = orderRepo;
            _drinkRepo = drinkRepo;
        }

        public IActionResult Index()
        {
            List<Order> orders = _orderRepo.GetAllOrders();
            return View(orders);
        }

        public IActionResult Create()
        {
            ViewModels.OrderViewModel model = new ViewModels.OrderViewModel
            {
                AllStudents = _orderRepo.GetAllStudents(),
                AllDrinks = _orderRepo.GetAllDrinks()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrder(int studentNumber, int drinkId, int quantity)
        {
            _orderRepo.AddOrder(studentNumber, drinkId, quantity);

            Drink? drink = _drinkRepo.GetById(drinkId);
            if (drink != null)
            {
                drink.Stock = drink.Stock - quantity;
                _drinkRepo.Update(drink);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveOrder(int studentNumber, int drinkId)
        {
            _orderRepo.RemoveOrder(studentNumber, drinkId);
            return RedirectToAction(nameof(Index));
        }
    }
}
