using Microsoft.Data.SqlClient;
using someren_vilage.Models;

namespace someren_vilage.Repositorie
{
    public class DbLecturerRepository : ILecturerRepository
    {

        private string studentColumnNames = "first_name, last_name, phone_number, age, room_id";

        private readonly String? _connectionString;
        public DbLecturerRepository(IConfiguration configuration)
        {
            // get (database) connection string from appsettings
            _connectionString = configuration.GetConnectionString("SomerenDb");
        }


        public Lecturer LecturerReader(SqlDataReader reader)
        {
            //int id = (int)reader["lecturer_id"];
            string lecturerFirstName = (string)reader["first_name"];
            string lecturerLastName = (string)reader["last_name"];
            string lecturerPhoneNumber = (string)reader["phone_number"];
            int lecturerAge = Convert.ToInt32(reader["age"]);
            int lecturerRoomId = (int)reader["room_id"];

            return new Lecturer(lecturerFirstName, lecturerLastName, lecturerPhoneNumber, lecturerAge, lecturerRoomId);
        }

        public List<Lecturer> GetAll()
        {
            List<Lecturer> lecturers = new List<Lecturer>();

            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"SELECT {studentColumnNames} FROM dbo.Lecturer";
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
            throw new NotImplementedException();
        }

        public void Delete(int lecturerId)
        {
            throw new NotImplementedException();
        }

        public Lecturer? GetById(int lecturerId)
        {
            throw new NotImplementedException();
        }

        public void Update(Lecturer lecturer)
        {
            throw new NotImplementedException();
        }
    }
}
