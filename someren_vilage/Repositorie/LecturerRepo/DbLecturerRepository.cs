using System.Data;
using Microsoft.Data.SqlClient;
using someren_vilage.Models;

namespace someren_vilage.Repositorie.LecturerRepo
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


        private Lecturer LecturerReader(SqlDataReader reader)
        {
            int id = (int)reader["lecturer_id"];
            string lecturerFirstName = (string)reader["first_name"];
            string lecturerLastName = (string)reader["last_name"];
            string lecturerPhoneNumber = (string)reader["phone_number"];
            int lecturerAge = Convert.ToInt32(reader["age"]);
            int? lecturerRoomId = reader["room_id"] as int?;

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
            try
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
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all lecturers.", ex);
            }
        }

        public void Add(Lecturer lecturer)
        {
            try
            {
                //throw new NotImplementedException();

                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        string query = "INSERT INTO dbo.Lecturer (first_name, last_name, phone_number, age, room_id) VALUES (@FirstName, @LastName, @PhoneNumber, @Age, @RoomId)";

                        SqlCommand command = new SqlCommand(query, connection);

                        command.Parameters.AddWithValue("@FirstName", lecturer.FirstName);
                        command.Parameters.AddWithValue("@LastName", lecturer.LastName);
                        command.Parameters.AddWithValue("@PhoneNumber", lecturer.PhoneNumber);
                        command.Parameters.AddWithValue("@Age", lecturer.Age);
                        command.Parameters.Add(new SqlParameter("@RoomId", SqlDbType.Int)
                        {
                            Value = (object?)lecturer.RoomId ?? DBNull.Value
                        });

                        command.Connection.Open();
                        int numberOfRowsAffected = command.ExecuteNonQuery();
                        if (numberOfRowsAffected != 1)
                        {
                            throw new Exception("Adding lecturer failed.");
                        }
                    }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding lecturer. {ex.Message}", ex);
            }
         }

        public void Delete(int lecturerId)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception($"Error deleting lecturer with ID {lecturerId}.", ex);
            }
        }

        public Lecturer? GetById(int lecturerId)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving lecturer with ID {lecturerId}.", ex);
            }
        }

        public void Update(Lecturer lecturer)
        {
            try
            {
                //throw new NotImplementedException();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = $"UPDATE dbo.Lecturer " + "SET first_name=@FirstName, last_name=@LastName, phone_number=@PhoneNumber, age=@Age, room_id=@RoomId WHERE lecturer_id = @lecturerId";
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@lecturerId", lecturer.LecturerId);
                    command.Parameters.AddWithValue("@FirstName", lecturer.FirstName);
                    command.Parameters.AddWithValue("@LastName", lecturer.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", lecturer.PhoneNumber);
                    command.Parameters.AddWithValue("@Age", lecturer.Age);
                    command.Parameters.Add(new SqlParameter("@roomId", SqlDbType.Int) 
{ 
    Value = (object?)lecturer.RoomId ?? DBNull.Value 
});


                    command.Connection.Open();

                    // checks
                    int numberOfRowsAffected = command.ExecuteNonQuery();
                    if (numberOfRowsAffected <= 0)
                        throw new Exception("No rows changed");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating lecturer. {ex.Message}", ex);
            }
        }
    }
}
