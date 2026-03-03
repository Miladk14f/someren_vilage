using System.Collections.Generic;

namespace someren_vilage.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        // FK
        public int StudentId { get; set; }
        public Student Student { get; set; }

        // Navigation
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
