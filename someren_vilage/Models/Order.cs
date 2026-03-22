using System.Collections.Generic;

namespace someren_vilage.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public int Quantity { get; set; }

        public int StudentNumber { get; set; }
        public Student Student { get; set; }

        public int DrinkId { get; set; }
        public Drink Drink { get; set; }
    }
}
