using someren_vilage.Models;

namespace someren_vilage.Repositorie.OrderRepo
{
    public interface IOrderRepository
    {
        List<Order> GetAllOrders();
        List<Order> GetOrders(int studentNumber);
        void AddOrder(int studentNumber, int drinkId, int quantity);
        void RemoveOrder(int studentNumber, int drinkId);
        List<Student> GetAllStudents();
        List<Drink> GetAllDrinks();
    }
}
