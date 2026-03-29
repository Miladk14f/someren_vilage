using System.Collections.Generic;
using someren_vilage.Models;

namespace someren_vilage.ViewModels
{
    public class RoomStudentsViewModel
    {
        public Room Room { get; set; }
        public List<Student> AssignedStudents { get; set; }
        public List<Student> UnassignedStudents { get; set; }
        public List<Lecturer> AssignedLecturers { get; set; }
        public List<Lecturer> UnassignedLecturers { get; set; }

        public RoomStudentsViewModel()
        {
            try
            {
                Room = new Room();
                AssignedStudents = new List<Student>();
                UnassignedStudents = new List<Student>();
                AssignedLecturers = new List<Lecturer>();
                UnassignedLecturers = new List<Lecturer>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing RoomStudentsViewModel.", ex);
            }
        }
    }
}
