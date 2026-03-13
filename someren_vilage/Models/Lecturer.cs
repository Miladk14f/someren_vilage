using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace someren_vilage.Models
{
    public class Lecturer
    {
        public int LecturerId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public byte? Age { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }

        public ICollection<LecturerActivity> Activities { get; set; }
    }
}
