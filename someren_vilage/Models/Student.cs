using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace someren_vilage.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        // FK
        public int RoomId { get; set; }
        public Room Room { get; set; }

        // Navigation
        public ICollection<ActivityParticipant> Activities { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
