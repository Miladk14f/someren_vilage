using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace someren_vilage.Models
{
    public class Activity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // FK (Lecturer guides activity)
        public int LecturerId { get; set; }
        public Lecturer Lecturer { get; set; }

        // Navigation
        public ICollection<ActivityParticipant> Participants { get; set; }
    }
}
