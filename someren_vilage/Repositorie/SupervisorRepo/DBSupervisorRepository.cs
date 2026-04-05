using Microsoft.Data.SqlClient;
using someren_vilage.Models;

namespace someren_vilage.Repositorie.SupervisorRepo
{
    public class DBSupervisorRepository : ISupervisorRepository
    {
        private string connectionString;

        public DBSupervisorRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("SomerenDb");
        }

        public void AddSupervisorToActivity(int activityId, int lecturerId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO dbo.LecturerActivity (lecturer_id, activity_id) VALUES (@lecturerId, @activityId)";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@lecturerId", lecturerId);
                command.Parameters.AddWithValue("@activityId", activityId);

                connection.Open();

                int numOfRowsEffected = command.ExecuteNonQuery();
                if (numOfRowsEffected != 1)
                {
                    throw new Exception("No records changed! somthing went wrong!");
                }
            }
        }

        public void DeleteSupervisorFromActivity(int activityId, int lecturerId)
        {
            //throw new NotImplementedException();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM dbo.LecturerActivity WHERE activity_id = @activityId AND lecturer_id = @lecturerId";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@activityId", activityId);
                command.Parameters.AddWithValue("@lecturerId", lecturerId);

                command.Connection.Open();

                int numOfRowsEffected = command.ExecuteNonQuery();
                if (numOfRowsEffected != 1)
                {
                    throw new Exception("No records changed! somthing went wrong!");
                }
            }
        }

        public List<Lecturer> GetSupervisors(int activityId)
        {
            //throw new NotImplementedException();

            List<Lecturer> supervisorsList = new List<Lecturer>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // get lect id, join on lecturer table to get object details, create lecturer object, and insurt into lecturer list
                string query = "SELECT l.lecturer_id, first_name, last_name, phone_number, age, room_id " +
                    "FROM dbo.Lecturer As l Join dbo.LecturerActivity as a ON a.lecturer_id = l.lecturer_id " +
                    "WHERE a.activity_id = @activityId";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@activityId", activityId);

                command.Connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    supervisorsList.Add(LecturerReader(reader));
                }

                return supervisorsList;
            }
        }


        private Lecturer LecturerReader(SqlDataReader reader)
        {
            int id = (int)reader["lecturer_id"];
            string lecturerFirstName = (string)reader["first_name"];
            string lecturerLastName = (string)reader["last_name"];
            string lecturerPhoneNumber = (string)reader["phone_number"];
            int lecturerAge = Convert.ToInt32(reader["age"]);
            int? lecturerRoomId = reader["room_id"] as int?;

            return new Lecturer
            {
                LecturerId = id,
                FirstName = lecturerFirstName,
                LastName = lecturerLastName,
                PhoneNumber = lecturerPhoneNumber,
                Age = lecturerAge,
                RoomId = lecturerRoomId
            };
        }
    } 
}
