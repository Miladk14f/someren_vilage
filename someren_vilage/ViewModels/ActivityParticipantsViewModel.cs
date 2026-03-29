using System.Collections.Generic;
using someren_vilage.Models;

namespace someren_vilage.ViewModels
{
    public class ActivityParticipantsViewModel
    {
        public Activity Activity { get; set; }
        public List<Student> Participants { get; set; }
        public List<Student> AllStudents { get; set; }

        public ActivityParticipantsViewModel()
        {
            Activity = new Activity();
            Participants = new List<Student>();
            AllStudents = new List<Student>();
        }
    }
}
