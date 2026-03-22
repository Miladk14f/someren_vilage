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

        public List<Student> GetParticipants(int activityId)
        {
            var list = new List<Student>();
            const string query = @"SELECT s.student_number, s.first_name, s.last_name, s.phone_number, s.class, s.room_id
FROM dbo.Student s
JOIN dbo.StudentActivity sa ON s.student_number = sa.student_number
WHERE sa.activity_id = @id";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = activityId });
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(ReadStudent(reader));
            }
            return list;
        }

        public void AddParticipant(int activityId, int studentNumber)
        {

            const string activityQuery = $"SELECT {ActivityColumns} FROM dbo.Activity WHERE activity_id = @aid";
            using var conn = new SqlConnection(connectionString);
            using var actCmd = new SqlCommand(activityQuery, conn);
            actCmd.Parameters.Add(new SqlParameter("@aid", SqlDbType.Int) { Value = activityId });
            conn.Open();
            using var reader = actCmd.ExecuteReader(CommandBehavior.SingleRow);
            if (!reader.Read())
            {
                throw new InvalidOperationException("Activity not found.");
            }

            var day = reader.IsDBNull(reader.GetOrdinal("day")) ? string.Empty : reader.GetString(reader.GetOrdinal("day"));
            TimeSpan time = TimeSpan.Zero;
            try 
            {
            time = reader.GetTimeSpan(reader.GetOrdinal("time_of_day")); } catch { try { time = reader.GetDateTime(reader.GetOrdinal("time_of_day")).TimeOfDay; } catch { }
            }
            reader.Close();

            const string conflictQuery = @"SELECT COUNT(*) FROM dbo.StudentActivity sa
JOIN dbo.Activity a ON sa.activity_id = a.activity_id
WHERE sa.student_number = @sno AND a.day = @day AND a.time_of_day = @time AND sa.activity_id <> @aid";
            using var conflictCmd = new SqlCommand(conflictQuery, conn);
            conflictCmd.Parameters.Add(new SqlParameter("@sno", SqlDbType.Int) { Value = studentNumber });
            conflictCmd.Parameters.Add(new SqlParameter("@day", SqlDbType.VarChar, 20) { Value = day });
            conflictCmd.Parameters.Add(new SqlParameter("@time", SqlDbType.Time) { Value = time });
            conflictCmd.Parameters.Add(new SqlParameter("@aid", SqlDbType.Int) { Value = activityId });
            var conflicts = (int)conflictCmd.ExecuteScalar();
            if (conflicts > 0)
                throw new InvalidOperationException("Student is already assigned to another activity at the same day and time.");
            const string insertQuery = "INSERT INTO dbo.StudentActivity(student_number, activity_id) VALUES(@sno, @aid)";
            using var insertCmd = new SqlCommand(insertQuery, conn);
            insertCmd.Parameters.Add(new SqlParameter("@sno", SqlDbType.Int) { Value = studentNumber });
            insertCmd.Parameters.Add(new SqlParameter("@aid", SqlDbType.Int) { Value = activityId });
            insertCmd.ExecuteNonQuery();
        }

        public void RemoveParticipant(int activityId, int studentNumber)
        {
            const string query = "DELETE FROM dbo.StudentActivity WHERE activity_id = @aid AND student_number = @sno";
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.Add(new SqlParameter("@aid", SqlDbType.Int) { Value = activityId });
            command.Parameters.Add(new SqlParameter("@sno", SqlDbType.Int) { Value = studentNumber });
            connection.Open();
            command.ExecuteNonQuery();
        }

        public List<Lecturer> GetLecturers(int activityId)
        {
            var list = new List<Lecturer>();
            const string query = @"SELECT l.lecturer_id, l.first_name, l.last_name, l.phone_number, l.age, l.room_id
FROM dbo.Lecturer l
JOIN dbo.LecturerActivity la ON l.lecturer_id = la.lecturer_id
WHERE la.activity_id = @id";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = activityId });
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read()) list.Add(ReadLecturer(reader));
            return list;
        }

        public void AddLecturer(int activityId, int lecturerId)
        {

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            const string activityQuery = $"SELECT {ActivityColumns} FROM dbo.Activity WHERE activity_id = @aid";
            using var actCmd = new SqlCommand(activityQuery, connection);
            actCmd.Parameters.Add(new SqlParameter("@aid", SqlDbType.Int) { Value = activityId });
            using var reader = actCmd.ExecuteReader(CommandBehavior.SingleRow);
            if (!reader.Read()) throw new InvalidOperationException("Activity not found.");
            var day = reader.IsDBNull(reader.GetOrdinal("day")) ? string.Empty : reader.GetString(reader.GetOrdinal("day"));
            TimeSpan time = TimeSpan.Zero;
            try { time = reader.GetTimeSpan(reader.GetOrdinal("time_of_day")); } catch { try { time = reader.GetDateTime(reader.GetOrdinal("time_of_day")).TimeOfDay; } catch { } }
            reader.Close();

            const string conflictQuery = @"SELECT COUNT(*) FROM dbo.LecturerActivity la
JOIN dbo.Activity a ON la.activity_id = a.activity_id
WHERE la.lecturer_id = @lid AND a.day = @day AND a.time_of_day = @time AND la.activity_id <> @aid";
            using var conflictCmd = new SqlCommand(conflictQuery, connection);
            conflictCmd.Parameters.Add(new SqlParameter("@lid", SqlDbType.Int) { Value = lecturerId });
            conflictCmd.Parameters.Add(new SqlParameter("@day", SqlDbType.VarChar, 20) { Value = day });
            conflictCmd.Parameters.Add(new SqlParameter("@time", SqlDbType.Time) { Value = time });
            conflictCmd.Parameters.Add(new SqlParameter("@aid", SqlDbType.Int) { Value = activityId });
            var conflicts = (int)conflictCmd.ExecuteScalar();
            if (conflicts > 0)
                throw new InvalidOperationException("Lecturer is already assigned to another activity at the same day and time.");
            const string insertQuery = "INSERT INTO dbo.LecturerActivity(lecturer_id, activity_id) VALUES(@lid, @aid)";
            using var insertCmd = new SqlCommand(insertQuery, connection);
            insertCmd.Parameters.Add(new SqlParameter("@lid", SqlDbType.Int) { Value = lecturerId });
            insertCmd.Parameters.Add(new SqlParameter("@aid", SqlDbType.Int) { Value = activityId });
            insertCmd.ExecuteNonQuery();
        }

        public void RemoveLecturer(int activityId, int lecturerId)
        {
            const string query = "DELETE FROM dbo.LecturerActivity WHERE activity_id = @aid AND lecturer_id = @lid";
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.Add(new SqlParameter("@aid", SqlDbType.Int) { Value = activityId });
            command.Parameters.Add(new SqlParameter("@lid", SqlDbType.Int) { Value = lecturerId });
            connection.Open();
            command.ExecuteNonQuery();
        }

        public List<Student> GetAllStudents()
        {
            var list = new List<Student>();
            const string query = "SELECT student_number, first_name, last_name, phone_number, class, room_id FROM dbo.Student";
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read()) list.Add(ReadStudent(reader));
            return list;
        }

        public List<Lecturer> GetAllLecturers()
        {
            var list = new List<Lecturer>();
            const string query = "SELECT lecturer_id, first_name, last_name, phone_number, age, room_id FROM dbo.Lecturer";
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read()) list.Add(ReadLecturer(reader));
            return list;
        }

        private static Student ReadStudent(SqlDataReader reader)
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

        private static Lecturer ReadLecturer(SqlDataReader reader)
        {
            int id = reader.GetInt32(reader.GetOrdinal("lecturer_id"));
            string firstname = GetStringOrEmpty(reader, "first_name");
            string lastname = GetStringOrEmpty(reader, "last_name");
            string phonenumber = GetStringOrEmpty(reader, "phone_number");
            byte? age = null;
            try { age = reader.IsDBNull(reader.GetOrdinal("age")) ? null : (byte?)reader.GetByte(reader.GetOrdinal("age")); } catch { }
            int roomid = reader.GetInt32(reader.GetOrdinal("room_id"));

            return new Lecturer()
            {
                LecturerId = id,
                FirstName = firstname,
                LastName = lastname,
                PhoneNumber = phonenumber,
                Age = age,
                RoomId = roomid
            };
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
            catch (SqlException ex) when (ex.Number == 547)
            {
                throw new InvalidOperationException(
                    "This activity cannot be deleted because it is currently assigned to one or more students or lecturers. " +
                    "Please remove them from this activity first.", ex);
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
