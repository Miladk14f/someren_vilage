using someren_vilage.Models;

namespace someren_vilage.Repositorie
{
    public interface ILecturerRepository 
    {
        List<Lecturer> GetAll();
        Lecturer? GetById(int lecturerId);
        void Add(Lecturer lecturer);
        void Update(Lecturer lecturer);
        void Delete(int lecturerId);
    }
}
