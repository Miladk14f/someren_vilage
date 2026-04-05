using someren_vilage.Models;

namespace someren_vilage.Repositorie.SupervisorRepo
{
    public interface ISupervisorRepository
    {
        void AddSupervisorToActivity(int activityId, int lecturerId);
        void DeleteSupervisorFromActivity(int activityId, int lecturerId);
        List<Lecturer> GetSupervisors(int activityId);
        //List<Lecturer> GetAllSupervisors(); // same as GetAllLecturer! redundent.
    }
}
