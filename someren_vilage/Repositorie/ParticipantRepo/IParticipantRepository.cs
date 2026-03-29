using someren_vilage.Models;

namespace someren_vilage.Repositorie.ParticipantRepo
{
    public interface IParticipantRepository
    {
        List<Student> GetParticipants(int activityId);
        void AddParticipant(int activityId, int studentNumber);
        void RemoveParticipant(int activityId, int studentNumber);
        List<Student> GetAllStudents();
    }
}
