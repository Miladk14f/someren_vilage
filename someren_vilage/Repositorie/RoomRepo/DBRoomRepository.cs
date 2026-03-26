using Microsoft.Data.SqlClient;
using someren_vilage.Models;

namespace someren_vilage.Repositorie.RoomRepo
{
    public class DbRoomRepository : IRoomRepository
    {
        private readonly string connectionString;

        public DbRoomRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("SomerenDb")
                ?? throw new InvalidOperationException("Connection string 'SomerenDb' not found in configuration.");
        }

        public List<Room> GetAll()
        {
            List<Room> rooms = new List<Room>();

            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand("SELECT room_id, floor, room_type, capacity, building_Name FROM dbo.Room", connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Room room = ReadRoom(reader);
                rooms.Add(room);
            }

            return rooms;
        }

        private Room ReadRoom(SqlDataReader reader)
        {
            Room room = new Room()
            {
                RoomId = (int)reader["room_id"],
                Floor = (int)reader["floor"],
                RoomType = (string)reader["room_type"],
                Capacity = (int)reader["capacity"],
                BuildingName = (string)reader["building_Name"]
            };
            return room;
        }

        public Room? GetById(int roomId)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand("SELECT room_id, floor, room_type, capacity, building_Name FROM dbo.Room WHERE room_id = @id", connection);
            command.Parameters.AddWithValue("@id", roomId);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return ReadRoom(reader);
            }

            return null;
        }

        public void Add(Room room)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "INSERT INTO dbo.Room (floor, room_type, capacity, building_Name) VALUES (@floor, @type, @capacity, @building); SELECT CAST(SCOPE_IDENTITY() AS int);", connection);

            command.Parameters.AddWithValue("@floor", room.Floor);
            command.Parameters.AddWithValue("@type", room.RoomType);
            command.Parameters.AddWithValue("@capacity", room.Capacity);
            command.Parameters.AddWithValue("@building", room.BuildingName);

            connection.Open();
            room.RoomId = (int)command.ExecuteScalar();
        }

        public void Update(Room room)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "UPDATE dbo.Room SET floor = @floor, room_type = @type, capacity = @capacity, building_Name = @building WHERE room_id = @id", connection);

            command.Parameters.AddWithValue("@floor", room.Floor);
            command.Parameters.AddWithValue("@type", room.RoomType);
            command.Parameters.AddWithValue("@capacity", room.Capacity);
            command.Parameters.AddWithValue("@building", room.BuildingName);
            command.Parameters.AddWithValue("@id", room.RoomId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Delete(int roomId)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand("DELETE FROM dbo.Room WHERE room_id = @id", connection);
            command.Parameters.AddWithValue("@id", roomId);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
