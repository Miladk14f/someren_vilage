using System.Collections.Generic;
using someren_vilage.Models;

namespace someren_vilage.ViewModels
{
    public class ActivitySupervisorsViewModel
    {
        public Activity Activity { get; set; }
        public List<Lecturer> Lecturers { get; set; }
        public List<Lecturer> AllLecturers { get; set; }

        public ActivitySupervisorsViewModel()
        {
            Activity = new Activity();
            Lecturers = new List<Lecturer>();
            AllLecturers = new List<Lecturer>();
        }
    }
}
