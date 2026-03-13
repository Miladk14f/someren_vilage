using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace someren_vilage.Models
{
    public class Student
    {
        public int StudentNumber { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        public string Class { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }

        public ICollection<ActivityParticipant> Activities { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
