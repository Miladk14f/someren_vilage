using Microsoft.AspNetCore.Mvc;
using someren_vilage.Models;
using someren_vilage.Repositorie.DrinkRepo;
using someren_vilage.Repositorie.OrderRepo;
using someren_vilage.Repositorie.StudentRepo;

namespace someren_vilage.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IDrinkRepository _drinkRepo;
        private readonly IStudentRepository _studentRepo;

        public OrderController(IOrderRepository orderRepo, IDrinkRepository drinkRepo, IStudentRepository studentRepo)
        {
            _orderRepo = orderRepo;
            _drinkRepo = drinkRepo;
            _studentRepo = studentRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                List<Order> orders = _orderRepo.GetAllOrders();
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<Order>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                ViewModels.OrderViewModel model = new ViewModels.OrderViewModel
                {
                    AllStudents = _studentRepo.GetAll(),
                    AllDrinks = _drinkRepo.GetAll()
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
        public IActionResult AddOrder(int studentNumber, int drinkId, int quantity)
        {
            try
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
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveOrder(int studentNumber, int drinkId)
        {
            try
            {
                _orderRepo.RemoveOrder(studentNumber, drinkId);
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
