using System.Data;
using Microsoft.Data.SqlClient;
using someren_vilage.Models;

namespace someren_vilage.Repositorie
{
    public class DbActivityRepository : IActivityRepository
    {
        private readonly string connectionString;

        private const string ActivityColumns = "activity_id, name AS Name, day AS Day, time_of_day AS TimeOfDay";

        public DbActivityRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("SomerenDb")
                ?? throw new InvalidOperationException("Connection string 'SomerenDb' not found in configuration.");
        }

        public List<Activity> GetAll()
        {
            var activities = new List<Activity>();
            string query = $"SELECT {ActivityColumns} FROM dbo.Activity";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    activities.Add(ReadActivity(reader));
                }

                return activities;
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

        private static Activity ReadActivity(SqlDataReader reader)
        {
            int id = reader.GetInt32(reader.GetOrdinal("activity_id"));
            string name = GetStringOrEmpty(reader, "Name");
            string day = GetStringOrEmpty(reader, "Day");
            TimeSpan time = GetTimeSpanOrZero(reader, "TimeOfDay");

            return new Activity
            {
                ActivityId = id,
                Name = name,
                Day = day,
                TimeOfDay = time
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

        private static TimeSpan GetTimeSpanOrZero(SqlDataReader reader, string columnName)
        {
            try
            {
                int ord = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(ord)) return TimeSpan.Zero;

                var fieldType = reader.GetFieldType(ord);
                if (fieldType == typeof(TimeSpan)) return reader.GetTimeSpan(ord);
                if (fieldType == typeof(DateTime)) return reader.GetDateTime(ord).TimeOfDay;
                if (fieldType == typeof(string))
                {
                    var s = reader.GetString(ord);
                    if (TimeSpan.TryParse(s, out var ts)) return ts;
                    if (DateTime.TryParse(s, out var dt)) return dt.TimeOfDay;
                }

                var val = reader.GetValue(ord);
                if (val is long l) return TimeSpan.FromSeconds(l);
                if (val is int i) return TimeSpan.FromSeconds(i);
                if (val is double d) return TimeSpan.FromSeconds(d);
                if (val is decimal dec) return TimeSpan.FromSeconds((double)dec);
            }
            catch (IndexOutOfRangeException)
            {
            }

            return TimeSpan.Zero;
        }

        public Activity? GetById(int activityId)
        {
            string query = $"SELECT {ActivityColumns} FROM dbo.Activity WHERE activity_id = @id";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = activityId });

            connection.Open();
            try
            {
                using var reader = command.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.Read())
                {
                    return ReadActivity(reader);
                }

                return null;
            }
            catch (SqlException ex)
            {
                string paramInfo = $"@id={activityId}";
                throw new InvalidOperationException($"SQL error executing GetById. Command: {command.CommandText}. Params: {paramInfo}", ex);
            }
        }

        public void Add(Activity activity)
        {
            const string query = @"
INSERT INTO dbo.Activity (name, day, time_of_day)
VALUES (@name, @day, @time);
SELECT CAST(SCOPE_IDENTITY() AS int);";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);

            if (string.IsNullOrWhiteSpace(activity.Name))
            {
                throw new ArgumentException("Activity.Name is required.", nameof(activity));
            }
            if (string.IsNullOrWhiteSpace(activity.Day))
            {
                throw new ArgumentException("Activity.Day is required.", nameof(activity));
            }

            command.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 200) { Value = activity.Name });
            command.Parameters.Add(new SqlParameter("@day", SqlDbType.VarChar, 20) { Value = activity.Day });
            command.Parameters.Add(new SqlParameter("@time", SqlDbType.Time) { Value = activity.TimeOfDay });

            try
            {
                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    try
                    {
                        activity.ActivityId = Convert.ToInt32(result);
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

        public void Update(Activity activity)
        {
            const string query = @"UPDATE dbo.Activity SET name = @name, day = @day, time_of_day = @time WHERE activity_id = @id";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);

            if (string.IsNullOrWhiteSpace(activity.Name))
            {
                throw new ArgumentException("Activity.Name is required.", nameof(activity));
            }
            if (string.IsNullOrWhiteSpace(activity.Day))
            {
                throw new ArgumentException("Activity.Day is required.", nameof(activity));
            }

            command.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 200) { Value = activity.Name });
            command.Parameters.Add(new SqlParameter("@day", SqlDbType.VarChar, 20) { Value = activity.Day });
            command.Parameters.Add(new SqlParameter("@time", SqlDbType.Time) { Value = activity.TimeOfDay });
            command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = activity.ActivityId });

            try
            {
                connection.Open();
                int rows = command.ExecuteNonQuery();
                if (rows == 0)
                {
                    throw new InvalidOperationException("Update failed: activity not found or no changes applied.");
                }
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"SQL error executing Update. Command: {command.CommandText} Params: @id={activity.ActivityId}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unexpected error executing Update.", ex);
            }
        }

        public void Delete(int activityId)
        {
            const string query = "DELETE FROM dbo.Activity WHERE activity_id = @id";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = activityId });

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"SQL error executing Delete. Command: {command.CommandText} Params: @id={activityId}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unexpected error executing Delete.", ex);
            }
        }
    }
}
