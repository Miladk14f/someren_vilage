using Microsoft.Data.SqlClient;
using someren_vilage.Models;

namespace someren_vilage.Repositorie.ParticipantRepo
{
    public class DbParticipantRepository : IParticipantRepository
    {
        private readonly string connectionString;

        public DbParticipantRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("SomerenDb")
                ?? throw new InvalidOperationException("Connection string 'SomerenDb' not found in configuration.");
        }

        public List<Student> GetParticipants(int activityId)
        {
            List<Student> list = new List<Student>();

            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "SELECT s.student_number, s.first_name, s.last_name, s.phone_number, s.class, s.room_id FROM dbo.Student s JOIN dbo.StudentActivity sa ON s.student_number = sa.student_number WHERE sa.activity_id = @id", connection);
            command.Parameters.AddWithValue("@id", activityId);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(ReadStudent(reader));
            }
            return list;
        }

        public void AddParticipant(int activityId, int studentNumber)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "INSERT INTO dbo.StudentActivity (student_number, activity_id) VALUES (@sno, @aid)", connection);
            command.Parameters.AddWithValue("@sno", studentNumber);
            command.Parameters.AddWithValue("@aid", activityId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void RemoveParticipant(int activityId, int studentNumber)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "DELETE FROM dbo.StudentActivity WHERE activity_id = @aid AND student_number = @sno", connection);
            command.Parameters.AddWithValue("@aid", activityId);
            command.Parameters.AddWithValue("@sno", studentNumber);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public List<Student> GetAllStudents()
        {
            List<Student> list = new List<Student>();

            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "SELECT student_number, first_name, last_name, phone_number, class, room_id FROM dbo.Student", connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(ReadStudent(reader));
            }
            return list;
        }

        private Student ReadStudent(SqlDataReader reader)
        {
            Student student = new Student()
            {
                StudentNumber = (int)reader["student_number"],
                FirstName = (string)reader["first_name"],
                LastName = (string)reader["last_name"],
                PhoneNumber = (string)reader["phone_number"],
                Class = (string)reader["class"],
                RoomId = reader["room_id"] as int?
            };
            return student;
        }
    }
}
