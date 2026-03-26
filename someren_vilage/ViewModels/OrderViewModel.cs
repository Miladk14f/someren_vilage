using someren_vilage.Models;

namespace someren_vilage.ViewModels
{
    public class OrderViewModel
    {
        public Student Student { get; set; }
        public List<Order> Orders { get; set; }
        public List<Drink> AllDrinks { get; set; }
        public List<Student> AllStudents { get; set; }
    }
}
