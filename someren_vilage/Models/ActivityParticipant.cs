namespace someren_vilage.Models
{
    public class ActivityParticipant
    {
        public int StudentNumber { get; set; }
        public Student Student { get; set; }

        public int ActivityId { get; set; }
        public Activity Activity { get; set; }
    }
}
