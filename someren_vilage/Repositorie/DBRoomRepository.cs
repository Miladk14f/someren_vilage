using someren_vilage.Models;

namespace someren_vilage.Repositorie
{
    public interface DBRoomRepository
    {
        List<Room> GetAll();
        Room? GetById(int roomid);
        void Add(Room room);
        void Update(Room room);
        void Delete(int roomid);
    }
}
