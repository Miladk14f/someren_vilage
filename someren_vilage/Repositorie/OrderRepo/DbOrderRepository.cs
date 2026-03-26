using Microsoft.Data.SqlClient;
using someren_vilage.Models;

namespace someren_vilage.Repositorie.OrderRepo
{
    public class DbOrderRepository : IOrderRepository
    {
        private readonly string connectionString;

        public DbOrderRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("SomerenDb")
                ?? throw new InvalidOperationException("Connection string 'SomerenDb' not found in configuration.");
        }

        public List<Order> GetAllOrders()
        {
            List<Order> list = new List<Order>();

            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "SELECT o.student_number, o.drink_id, o.quantity, s.first_name, s.last_name, d.name, d.price, d.vat_percentage, d.stock, d.alcoholic FROM dbo.[Order] o JOIN dbo.Student s ON o.student_number = s.student_number JOIN dbo.Drink d ON o.drink_id = d.drink_id", connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Order order = new Order()
                {
                    StudentNumber = (int)reader["student_number"],
                    DrinkId = (int)reader["drink_id"],
                    Quantity = (int)reader["quantity"],
                    Student = new Student()
                    {
                        StudentNumber = (int)reader["student_number"],
                        FirstName = (string)reader["first_name"],
                        LastName = (string)reader["last_name"]
                    },
                    Drink = new Drink()
                    {
                        DrinkId = (int)reader["drink_id"],
                        Name = (string)reader["name"],
                        Price = (decimal)reader["price"],
                        VatPercentage = (byte)reader["vat_percentage"],
                        Stock = (int)reader["stock"],
                        Alcoholic = (bool)reader["alcoholic"]
                    }
                };
                list.Add(order);
            }
            return list;
        }

        public List<Order> GetOrders(int studentNumber)
        {
            List<Order> list = new List<Order>();

            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "SELECT o.student_number, o.drink_id, o.quantity, d.name, d.price, d.vat_percentage, d.stock, d.alcoholic FROM dbo.[Order] o JOIN dbo.Drink d ON o.drink_id = d.drink_id WHERE o.student_number = @sno", connection);
            command.Parameters.AddWithValue("@sno", studentNumber);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(ReadOrder(reader));
            }
            return list;
        }

        public void AddOrder(int studentNumber, int drinkId, int quantity)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "INSERT INTO dbo.[Order] (student_number, drink_id, quantity) VALUES (@sno, @did, @qty)", connection);
            command.Parameters.AddWithValue("@sno", studentNumber);
            command.Parameters.AddWithValue("@did", drinkId);
            command.Parameters.AddWithValue("@qty", quantity);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void RemoveOrder(int studentNumber, int drinkId)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "DELETE FROM dbo.[Order] WHERE student_number = @sno AND drink_id = @did", connection);
            command.Parameters.AddWithValue("@sno", studentNumber);
            command.Parameters.AddWithValue("@did", drinkId);

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

        public List<Drink> GetAllDrinks()
        {
            List<Drink> list = new List<Drink>();

            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(
                "SELECT drink_id, name, price, vat_percentage, stock, alcoholic FROM dbo.Drink", connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(ReadDrink(reader));
            }
            return list;
        }

        private Order ReadOrder(SqlDataReader reader)
        {
            Order order = new Order()
            {
                StudentNumber = (int)reader["student_number"],
                DrinkId = (int)reader["drink_id"],
                Quantity = (int)reader["quantity"],
                Drink = new Drink()
                {
                    DrinkId = (int)reader["drink_id"],
                    Name = (string)reader["name"],
                    Price = (decimal)reader["price"],
                    VatPercentage = (byte)reader["vat_percentage"],
                    Stock = (int)reader["stock"],
                    Alcoholic = (bool)reader["alcoholic"]
                }
            };
            return order;
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
                RoomId = (int)reader["room_id"]
            };
            return student;
        }

        private Drink ReadDrink(SqlDataReader reader)
        {
            Drink drink = new Drink()
            {
                DrinkId = (int)reader["drink_id"],
                Name = (string)reader["name"],
                Price = (decimal)reader["price"],
                VatPercentage = (byte)reader["vat_percentage"],
                Stock = (int)reader["stock"],
                Alcoholic = (bool)reader["alcoholic"]
            };
            return drink;
        }
    }
}
