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
            try
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
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all rooms.", ex);
            }
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
            try
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
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving room with ID {roomId}.", ex);
            }
        }

        public void Add(Room room)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception("Error adding room.", ex);
            }
        }

        public void Update(Room room)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception("Error updating room.", ex);
            }
        }

        public void Delete(int roomId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(connectionString);
                using SqlCommand command = new SqlCommand("DELETE FROM dbo.Room WHERE room_id = @id", connection);
                command.Parameters.AddWithValue("@id", roomId);

                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting room with ID {roomId}.", ex);
            }
        }
    }
}
