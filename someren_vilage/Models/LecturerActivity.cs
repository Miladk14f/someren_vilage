namespace someren_vilage.Models
{
    public class LecturerActivity
    {
        public int LecturerId { get; set; }
        public Lecturer Lecturer { get; set; }

        public int ActivityId { get; set; }
        public Activity Activity { get; set; }
    }
}
