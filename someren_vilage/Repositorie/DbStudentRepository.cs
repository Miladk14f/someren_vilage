using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using someren_vilage.Models;

namespace someren_vilage.Repositorie
{
    public class DbStudentRepository : IStudentRepository
    {
        private readonly string connectionString;

        private const string StudentColumns =
            "student_number, first_name, last_name, phone_number, class, room_id";
        
        public DbStudentRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("SomerenDb")
                               ?? throw new InvalidOperationException("Connection string 'SomerenDb' not found in configuration.");
        }

        public List<Student> GetAll()
        {
            var student = new List<Student>();
            const string query = $"SELECT {StudentColumns} FROM dbo.Student ORDER BY last_name ASC";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);
            
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                student.Add(ReadStudent(reader));
            }

            return student;
        }

        public Student? GetById(int studentid)
        {
            const string query = $"SELECT {StudentColumns} FROM dbo.Student WHERE student_number = @Id";
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", studentid);
            
            connection.Open();
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return ReadStudent(reader);
            }

            return null;
        }

        public void Add(Student student)
        {
            const string query = "INSERT INTO dbo.Student (student_number, first_name, last_name, phone_number, class, room_id) " +
                                 "VALUES (@StudentNumber, @FirstName, @LastName, @PhoneNumber, @Class, @RoomId)";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@StudentNumber", student.StudentNumber);
            command.Parameters.AddWithValue("@FirstName", student.FirstName);
            command.Parameters.AddWithValue("@LastName", student.LastName);
            command.Parameters.AddWithValue("@PhoneNumber", (object)student.PhoneNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@Class", student.Class);
            command.Parameters.AddWithValue("@RoomId", student.RoomId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Update(Student student)
        {
                const string query = "UPDATE dbo.Student SET first_name = @FirstName, last_name = @LastName, " +
                                     "phone_number = @PhoneNumber, class = @Class, room_id = @RoomId " +
                                     "WHERE student_number = @StudentNumber";

                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@StudentNumber", student.StudentNumber);
                command.Parameters.AddWithValue("@FirstName", student.FirstName);
                command.Parameters.AddWithValue("@LastName", student.LastName);
                command.Parameters.AddWithValue("@PhoneNumber", (object)student.PhoneNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@Class", student.Class);
                command.Parameters.AddWithValue("@RoomId", student.RoomId);

                connection.Open();
                command.ExecuteNonQuery();
        }

        public void Delete(int studentid)
        {
            const string query = "DELETE FROM dbo.Student WHERE student_number = @Id";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);
    
            command.Parameters.AddWithValue("@Id", studentid);

            connection.Open();
            command.ExecuteNonQuery();
        }

        private static Student ReadStudent (SqlDataReader reader)
        {
            int studentnumber = reader.GetInt32(reader.GetOrdinal("student_number"));
            string firstname = GetStringOrEmpty(reader, "first_name");
            string lastname = GetStringOrEmpty(reader, "last_name");
            string phonenumber = GetStringOrEmpty(reader, "phone_number");
            string class1 = GetStringOrEmpty(reader, "class");
            int roomid = reader.GetInt32(reader.GetOrdinal("room_id"));

            return new Student()
            {
                StudentNumber = studentnumber,
                FirstName = firstname,
                LastName = lastname,
                PhoneNumber = phonenumber,
                Class = class1,
                RoomId = roomid
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
    }
}