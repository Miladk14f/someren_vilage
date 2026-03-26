using someren_vilage.Models;

namespace someren_vilage.Repositorie.SupervisorRepo
{
    public interface ISupervisorRepository
    {
        List<Lecturer> GetSupervisors(int activityId);
        void AddSupervisor(int activityId, int lecturerId);
        void RemoveSupervisor(int activityId, int lecturerId);
        List<Lecturer> GetAllLecturers();
    }
}
