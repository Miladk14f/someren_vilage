using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace someren_vilage.Models
{
    public class Activity
    {
        public int ActivityId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Day { get; set; }

        public TimeSpan TimeOfDay { get; set; }

        public ICollection<ActivityParticipant> Participants { get; set; }
        public ICollection<LecturerActivity> Lecturers { get; set; }
    }
}
