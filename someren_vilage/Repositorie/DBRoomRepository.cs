using System.Data;
using Microsoft.Data.SqlClient;
using someren_vilage.Models;

namespace someren_vilage.Repositorie
{
    public class DbRoomRepository : IRoomRepository
    {
        private readonly string connectionString;

        private const string RoomColumns = "room_id, floor AS Floor, room_type AS RoomType, capacity AS Capacity, building_Name AS BuildingName";

        public DbRoomRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("SomerenDb")
                ?? throw new InvalidOperationException("Connection string 'SomerenDb' not found in configuration.");
        }

        public List<Room> GetAll()
        {
            var rooms = new List<Room>();
            const string query =
                "SELECT room_id, floor AS Floor, room_type AS RoomType, capacity AS Capacity, building_Name AS BuildingName FROM dbo.Room";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    rooms.Add(ReadRoom(reader));
                }

                return rooms;
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"SQL error executing GetAll. Command: {command.CommandText}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unexpected error executing GetAll.", ex);
            }
        }

        private static Room ReadRoom(SqlDataReader reader)
        {
            int id = reader.GetInt32(reader.GetOrdinal("room_id"));
            int floor = reader.IsDBNull(reader.GetOrdinal("Floor")) ? 0 : reader.GetInt32(reader.GetOrdinal("Floor"));
            string type = GetStringOrEmpty(reader, "RoomType");
            int capacity = reader.IsDBNull(reader.GetOrdinal("Capacity")) ? 0 : reader.GetInt32(reader.GetOrdinal("Capacity"));
            string building = GetStringOrEmpty(reader, "BuildingName");

            return new Room
            {
                RoomId = id,
                Floor = floor,
                RoomType = type,
                Capacity = capacity,
                BuildingName = building
            };
        }

        private static string GetStringOrEmpty(SqlDataReader reader, string columnName)
        {
            try
            {
                int ord = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ord) ? string.Empty : reader.GetString(ord);
            }
            catch (IndexOutOfRangeException)
            {
                return string.Empty;
            }
        }

        public Room? GetById(int roomId)
        {
            string query = $"SELECT {RoomColumns} FROM dbo.Room WHERE room_id = @id";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = roomId });

            connection.Open();
            try
            {
                using var reader = command.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.Read())
                {
                    return ReadRoom(reader);
                }

                return null;
            }
            catch (SqlException ex)
            {
                string paramInfo = $"@id={roomId}";
                throw new InvalidOperationException($"SQL error executing GetById. Command: {command.CommandText}. Params: {paramInfo}", ex);
            }
        }

        public void Add(Room room)
        {
            const string query = @"
INSERT INTO dbo.Room (floor, room_type, capacity, building_Name)
VALUES (@floor, @type, @capacity, @building);
SELECT CAST(SCOPE_IDENTITY() AS int);";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.Add(new SqlParameter("@floor", SqlDbType.Int) { Value = room.Floor });
            command.Parameters.Add(new SqlParameter("@type", SqlDbType.NVarChar, 200) { Value = (object?)room.RoomType ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@capacity", SqlDbType.Int) { Value = room.Capacity });
            command.Parameters.Add(new SqlParameter("@building", SqlDbType.NVarChar, 200) { Value = (object?)room.BuildingName ?? DBNull.Value });

            try
            {
                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    try
                    {
                        room.RoomId = Convert.ToInt32(result);
                    }
                    catch
                    {
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"SQL error executing Add. Command: {command.CommandText}.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unexpected error executing Add.", ex);
            }
        }

        public void Update(Room room)
        {
            const string query = @"UPDATE dbo.Room
SET floor = @floor,
    room_type = @type,
    capacity = @capacity,
    building_Name = @building
WHERE room_id = @id";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.Add(new SqlParameter("@floor", SqlDbType.Int) { Value = room.Floor });
            command.Parameters.Add(new SqlParameter("@type", SqlDbType.NVarChar, 200) { Value = (object?)room.RoomType ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@capacity", SqlDbType.Int) { Value = room.Capacity });
            command.Parameters.Add(new SqlParameter("@building", SqlDbType.NVarChar, 200) { Value = (object?)room.BuildingName ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = room.RoomId });

            try
            {
                connection.Open();
                int rows = command.ExecuteNonQuery();
                if (rows == 0)
                {
                    throw new InvalidOperationException("Update failed: room not found or no changes applied.");
                }
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"SQL error executing Update. Command: {command.CommandText} Params: @id={room.RoomId}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unexpected error executing Update.", ex);
            }
        }

        public void Delete(int roomId)
        {
            const string query = "DELETE FROM dbo.Room WHERE room_id = @id";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = roomId });

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"SQL error executing Delete. Command: {command.CommandText} Params: @id={roomId}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unexpected error executing Delete.", ex);
            }
        }
    }
}
