using System.Collections.Generic;
using someren_vilage.Models;

namespace someren_vilage.ViewModels
{
    public class ActivityParticipantsViewModel
    {
        public Activity Activity { get; set; }
        public List<Student> Participants { get; set; }
        public List<Lecturer> Lecturers { get; set; }
        public List<Student> AllStudents { get; set; }
        public List<Lecturer> AllLecturers { get; set; }

        public ActivityParticipantsViewModel()
        {
            try
            {
                Activity = new Activity();
                Participants = new List<Student>();
                Lecturers = new List<Lecturer>();
                AllStudents = new List<Student>();
                AllLecturers = new List<Lecturer>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing ActivityParticipantsViewModel.", ex);
            }
        }
    }
}
