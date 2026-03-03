using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace someren_vilage.Models
{
    public class Lecturer
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Email { get; set; }

        // FK
        public int RoomId { get; set; }
        public Room Room { get; set; }

        // Navigation
        public ICollection<Activity> Activities { get; set; }
    }
}
