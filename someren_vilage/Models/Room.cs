using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace someren_vilage.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        public string RoomNumber { get; set; }

        public int Capacity { get; set; }

        // Navigation
        public ICollection<Student> Students { get; set; }
        public ICollection<Lecturer> Lecturers { get; set; }
    }
}
