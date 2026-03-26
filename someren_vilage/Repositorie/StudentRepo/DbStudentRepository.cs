using Microsoft.Data.SqlClient;
using someren_vilage.Models;

namespace someren_vilage.Repositorie.StudentRepo
{
    public class DbStudentRepository : IStudentRepository
    {
        private readonly string connectionString;

        public DbStudentRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("SomerenDb")
                ?? throw new InvalidOperationException("Connection string 'SomerenDb' not found in configuration.");
        }

        public List<Student> GetAll()
        {
            List<Student> students = new List<Student>();

            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand("SELECT student_number, first_name, last_name, phone_number, class, room_id FROM dbo.Student ORDER BY last_name ASC", connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Student student = ReadStudent(reader);
                students.Add(student);
            }

            return students;
        }

        private Student ReadStudent(SqlDataReader reader)
        {
            Student student = new Student()
            {
                StudentNumber = (int)reader["student_number"],
                FirstName = (string)reader["first_name"],
                LastName = (string)reader["last_name"],
                PhoneNumber = reader["phone_number"] as string ?? string.Empty,
                Class = (string)reader["class"],
                RoomId = (int)reader["room_id"]
            };
            return student;
        }

        public Student? GetById(int studentId)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand("SELECT student_number, first_name, last_name, phone_number, class, room_id FROM dbo.Student WHERE student_number = @id", connection);
            command.Parameters.AddWithValue("@id", studentId);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return ReadStudent(reader);
            }

            return null;
        }

        public void Add(Student student)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "INSERT INTO dbo.Student (student_number, first_name, last_name, phone_number, class, room_id) VALUES (@studentNumber, @firstName, @lastName, @phoneNumber, @class, @roomId)", connection);

            command.Parameters.AddWithValue("@studentNumber", student.StudentNumber);
            command.Parameters.AddWithValue("@firstName", student.FirstName);
            command.Parameters.AddWithValue("@lastName", student.LastName);
            command.Parameters.AddWithValue("@phoneNumber", (object)student.PhoneNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@class", student.Class);
            command.Parameters.AddWithValue("@roomId", student.RoomId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Update(Student student)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "UPDATE dbo.Student SET first_name = @firstName, last_name = @lastName, phone_number = @phoneNumber, class = @class, room_id = @roomId WHERE student_number = @studentNumber", connection);

            command.Parameters.AddWithValue("@firstName", student.FirstName);
            command.Parameters.AddWithValue("@lastName", student.LastName);
            command.Parameters.AddWithValue("@phoneNumber", (object)student.PhoneNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@class", student.Class);
            command.Parameters.AddWithValue("@roomId", student.RoomId);
            command.Parameters.AddWithValue("@studentNumber", student.StudentNumber);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Delete(int studentId)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand("DELETE FROM dbo.Student WHERE student_number = @id", connection);
            command.Parameters.AddWithValue("@id", studentId);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}