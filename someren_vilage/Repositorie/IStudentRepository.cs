using System.Collections.Generic;
using someren_vilage.Models;

namespace someren_vilage.Repositorie
{
    public interface IStudentRepository
    {
        List<Student> GetAll();
        Student? GetById(int studentid);
        void Add(Student student);
        void Update(Student student);
        void Delete(int studentid);
        List<Room> GetAllRooms();
    }
}