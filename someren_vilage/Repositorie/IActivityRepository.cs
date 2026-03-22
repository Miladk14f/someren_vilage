using System.Collections.Generic;
using someren_vilage.Models;

namespace someren_vilage.Repositorie
{
    public interface IActivityRepository
    {
        List<Activity> GetAll();
        Activity? GetById(int activityId);
        void Add(Activity activity);
        void Update(Activity activity);
        void Delete(int activityId);
        List<Models.Student> GetParticipants(int activityId);
        void AddParticipant(int activityId, int studentNumber);
        void RemoveParticipant(int activityId, int studentNumber);

        List<Models.Lecturer> GetLecturers(int activityId);
        void AddLecturer(int activityId, int lecturerId);
        void RemoveLecturer(int activityId, int lecturerId);

        List<Models.Student> GetAllStudents();
        List<Models.Lecturer> GetAllLecturers();
    }
}
