using Microsoft.Data.SqlClient;
using someren_vilage.Models;

namespace someren_vilage.Repositorie.SupervisorRepo
{
    public class DbSupervisorRepository : ISupervisorRepository
    {
        private readonly string connectionString;

        public DbSupervisorRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("SomerenDb")
                ?? throw new InvalidOperationException("Connection string 'SomerenDb' not found in configuration.");
        }

        public List<Lecturer> GetSupervisors(int activityId)
        {
            List<Lecturer> list = new List<Lecturer>();

            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "SELECT l.lecturer_id, l.first_name, l.last_name, l.phone_number, l.age, l.room_id FROM dbo.Lecturer l JOIN dbo.LecturerActivity la ON l.lecturer_id = la.lecturer_id WHERE la.activity_id = @id", connection);
            command.Parameters.AddWithValue("@id", activityId);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(ReadLecturer(reader));
            }
            return list;
        }

        public void AddSupervisor(int activityId, int lecturerId)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "INSERT INTO dbo.LecturerActivity (lecturer_id, activity_id) VALUES (@lid, @aid)", connection);
            command.Parameters.AddWithValue("@lid", lecturerId);
            command.Parameters.AddWithValue("@aid", activityId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void RemoveSupervisor(int activityId, int lecturerId)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "DELETE FROM dbo.LecturerActivity WHERE activity_id = @aid AND lecturer_id = @lid", connection);
            command.Parameters.AddWithValue("@aid", activityId);
            command.Parameters.AddWithValue("@lid", lecturerId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public List<Lecturer> GetAllLecturers()
        {
            List<Lecturer> list = new List<Lecturer>();

            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "SELECT lecturer_id, first_name, last_name, phone_number, age, room_id FROM dbo.Lecturer", connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(ReadLecturer(reader));
            }
            return list;
        }

        private Lecturer ReadLecturer(SqlDataReader reader)
        {
            Lecturer lecturer = new Lecturer()
            {
                LecturerId = (int)reader["lecturer_id"],
                FirstName = (string)reader["first_name"],
                LastName = (string)reader["last_name"],
                PhoneNumber = (string)reader["phone_number"],
                Age = Convert.ToInt32(reader["age"]),
                RoomId = reader["room_id"] as int?
            };
            return lecturer;
        }
    }
}
