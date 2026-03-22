using System.ComponentModel.DataAnnotations;

namespace someren_vilage.Models
{
    public class Drink
    {
        public int DrinkId { get; set; }

        [Required]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public byte VatPercentage { get; set; }

        public int Stock { get; set; }

        public bool Alcoholic { get; set; }
    }
}
