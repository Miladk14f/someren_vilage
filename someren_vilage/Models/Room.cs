using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace someren_vilage.Models
{
    public class Room
    {
        public int RoomId { get; set; }

        public int Floor { get; set; }

        [Required(ErrorMessage = "Room type is required")]
        [Display(Name = "Room Type")]
        [StringLength(50)]
        public string RoomType { get; set; }

        public int Capacity { get; set; }

        [Required(ErrorMessage = "Building is required")]
        [Display(Name = "Building")]
        [StringLength(10)]
        public string BuildingName { get; set; }
    }
}
