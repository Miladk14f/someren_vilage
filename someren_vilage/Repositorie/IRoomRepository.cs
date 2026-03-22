using System.Collections.Generic;
using someren_vilage.Models;

namespace someren_vilage.Repositorie
{
    public interface IRoomRepository
    {
        List<Room> GetAll();
        Room? GetById(int roomid);
        void Add(Room room);
        void Update(Room room);
        void Delete(int roomid);
    }
}
