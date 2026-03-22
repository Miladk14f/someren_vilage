using Microsoft.Data.SqlClient;
using someren_vilage.Models;

namespace someren_vilage.Repositorie
{
    public class DbLecturerRepository : ILecturerRepository
    {

        private string studentColumnNames = "lecturer_id, first_name, last_name, phone_number, age, room_id";

        private readonly String? _connectionString;
        public DbLecturerRepository(IConfiguration configuration)
        {
            // get (database) connection string from appsettings
            _connectionString = configuration.GetConnectionString("SomerenDb");
        }


        public Lecturer LecturerReader(SqlDataReader reader)
        {
            int id = (int)reader["lecturer_id"];
            string lecturerFirstName = (string)reader["first_name"];
            string lecturerLastName = (string)reader["last_name"];
            string lecturerPhoneNumber = (string)reader["phone_number"];
            int lecturerAge = Convert.ToInt32(reader["age"]);
            int lecturerRoomId = (int)reader["room_id"];

            //return new Lecturer(id, lecturerFirstName, lecturerLastName, lecturerPhoneNumber, lecturerAge, lecturerRoomId);

            return new Lecturer {
                LecturerId = id,
                FirstName = lecturerFirstName,
                LastName = lecturerLastName,
                PhoneNumber = lecturerPhoneNumber,
                Age = lecturerAge,
                RoomId = lecturerRoomId
            };
        }

        public List<Lecturer> GetAll()
        {
            List<Lecturer> lecturers = new List<Lecturer>();

            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"SELECT {studentColumnNames} FROM dbo.Lecturer ORDER BY last_name ASC";
                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // while loop to create object using LecturerReader
                while (reader.Read())
                {
                    Lecturer lecturer = LecturerReader(reader);
                    lecturers.Add(lecturer);
                }

                return lecturers;
            }
            
        }

        public void Add(Lecturer lecturer)
        {
            //throw new NotImplementedException();
            
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = $"INSERT INTO dbo.Lecturer (first_name, last_name, phone_number, age, room_id)" + 
                        $"VALUES ('{lecturer.FirstName}', '{lecturer.LastName}', '{lecturer.PhoneNumber}', '{lecturer.Age}', '{lecturer.RoomId}')";
                    
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Connection.Open();
                    int numberOfRowsAffected = command.ExecuteNonQuery();
                    if (numberOfRowsAffected != 1)
                    {
                        throw new Exception("Adding lecturer failed.");
                    }
                }
         }

        public void Delete(int lecturerId)
        {
            //throw new NotImplementedException();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"DELETE FROM dbo.Lecturer WHERE lecturer_id = @lecturerId";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@lecturerId", lecturerId);

                command.Connection.Open();

                // checks
                int numberOfRowsAffected = command.ExecuteNonQuery();
                if (numberOfRowsAffected <= 0)
                {
                    throw new Exception("No rows changed");
                }
            }
        }

        public Lecturer? GetById(int lecturerId)
        {
            //throw new NotImplementedException();
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"SELECT {studentColumnNames} FROM dbo.Lecturer WHERE lecturer_id = @lecturerId";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@lecturerId", lecturerId);

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return LecturerReader(reader);
                }

                return null;
            }
        }

        public void Update(Lecturer lecturer)
        {
            //throw new NotImplementedException();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"UPDATE dbo.Lecturer " + "SET first_name=@FirstName, last_name=@LastName, phone_number=@PhoneNumber, age=@Age WHERE lecturer_id = @lecturerId";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@lecturerId", lecturer.LecturerId);
                command.Parameters.AddWithValue("@FirstName", lecturer.FirstName);
                command.Parameters.AddWithValue("@LastName", lecturer.LastName);
                command.Parameters.AddWithValue("@PhoneNumber", lecturer.PhoneNumber);
                command.Parameters.AddWithValue("@Age", lecturer.Age);


                command.Connection.Open();

                // checks
                int numberOfRowsAffected = command.ExecuteNonQuery();
                if (numberOfRowsAffected <= 0)
                    throw new Exception("No rows changed");
            }

        }
    }
}
