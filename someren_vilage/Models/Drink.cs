using System.ComponentModel.DataAnnotations;

namespace someren_vilage.Models
{
    public class Drink
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public bool IsAlcoholic { get; set; }

        public int Stock { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
