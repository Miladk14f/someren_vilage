using someren_vilage.Models;

namespace someren_vilage.ViewModels
{
    public class OrderViewModel
    {
        public Student Student { get; set; }
        public List<Order> Orders { get; set; }
        public List<Drink> AllDrinks { get; set; }
        public List<Student> AllStudents { get; set; }

        public OrderViewModel()
        {
            try
            {
                Student = new Student();
                Orders = new List<Order>();
                AllDrinks = new List<Drink>();
                AllStudents = new List<Student>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing OrderViewModel.", ex);
            }
        }
    }
}
