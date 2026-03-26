using Microsoft.Data.SqlClient;
using someren_vilage.Models;

namespace someren_vilage.Repositorie.ActivityRepo
{
    public class DbActivityRepository : IActivityRepository
    {
        private readonly string connectionString;

        public DbActivityRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("SomerenDb")
                ?? throw new InvalidOperationException("Connection string 'SomerenDb' not found in configuration.");
        }

        public List<Activity> GetAll()
        {
            List<Activity> activities = new List<Activity>();

            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand("SELECT activity_id, name, day, time_of_day FROM dbo.Activity", connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Activity activity = ReadActivity(reader);
                activities.Add(activity);
            }

            return activities;
        }

        private Activity ReadActivity(SqlDataReader reader)
        {
            Activity activity = new Activity()
            {
                ActivityId = (int)reader["activity_id"],
                Name = (string)reader["name"],
                Day = (string)reader["day"],
                TimeOfDay = (TimeSpan)reader["time_of_day"]
            };
            return activity;
        }

        public Activity? GetById(int activityId)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand("SELECT activity_id, name, day, time_of_day FROM dbo.Activity WHERE activity_id = @id", connection);
            command.Parameters.AddWithValue("@id", activityId);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return ReadActivity(reader);
            }

            return null;
        }

        public void Add(Activity activity)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "INSERT INTO dbo.Activity (name, day, time_of_day) VALUES (@name, @day, @time); SELECT CAST(SCOPE_IDENTITY() AS int);", connection);

            command.Parameters.AddWithValue("@name", activity.Name);
            command.Parameters.AddWithValue("@day", activity.Day);
            command.Parameters.AddWithValue("@time", activity.TimeOfDay);

            connection.Open();
            activity.ActivityId = (int)command.ExecuteScalar();
        }

        public void Update(Activity activity)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "UPDATE dbo.Activity SET name = @name, day = @day, time_of_day = @time WHERE activity_id = @id", connection);

            command.Parameters.AddWithValue("@name", activity.Name);
            command.Parameters.AddWithValue("@day", activity.Day);
            command.Parameters.AddWithValue("@time", activity.TimeOfDay);
            command.Parameters.AddWithValue("@id", activity.ActivityId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Delete(int activityId)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand("DELETE FROM dbo.Activity WHERE activity_id = @id", connection);
            command.Parameters.AddWithValue("@id", activityId);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
