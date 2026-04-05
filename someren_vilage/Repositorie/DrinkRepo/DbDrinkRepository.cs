using Microsoft.Data.SqlClient;
using someren_vilage.Models;

namespace someren_vilage.Repositorie.DrinkRepo
{
    public class DbDrinkRepository : IDrinkRepository
    {
        private readonly string connectionString;

        public DbDrinkRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("SomerenDb")
                ?? throw new InvalidOperationException("Connection string 'SomerenDb' not found in configuration.");
        }

        public List<Drink> GetAll()
        {
            try
            {
                List<Drink> drinks = new List<Drink>();

                using SqlConnection connection = new SqlConnection(connectionString);
                using SqlCommand command = new SqlCommand(
                    "SELECT drink_id, name, price, vat_percentage, stock, alcoholic FROM dbo.Drink", connection);

                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Drink drink = ReadDrink(reader);
                    drinks.Add(drink);
                }

                return drinks;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all drinks.", ex);
            }
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

        public Drink? GetById(int drinkId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(connectionString);
                using SqlCommand command = new SqlCommand(
                    "SELECT drink_id, name, price, vat_percentage, stock, alcoholic FROM dbo.Drink WHERE drink_id = @id", connection);
                command.Parameters.AddWithValue("@id", drinkId);

                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return ReadDrink(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving drink with ID {drinkId}.", ex);
            }
        }

        public void Add(Drink drink)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(connectionString);
                using SqlCommand command = new SqlCommand(
                    "INSERT INTO dbo.Drink (name, price, vat_percentage, stock, alcoholic) VALUES (@name, @price, @vat, @stock, @alcoholic); SELECT CAST(SCOPE_IDENTITY() AS int);", connection);

                command.Parameters.AddWithValue("@name", drink.Name);
                command.Parameters.AddWithValue("@price", drink.Price);
                command.Parameters.AddWithValue("@vat", drink.VatPercentage);
                command.Parameters.AddWithValue("@stock", drink.Stock);
                command.Parameters.AddWithValue("@alcoholic", drink.Alcoholic);

                connection.Open();
                drink.DrinkId = (int)command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding drink.", ex);
            }
        }

        public void Update(Drink drink)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(connectionString);
                using SqlCommand command = new SqlCommand(
                    "UPDATE dbo.Drink SET name = @name, price = @price, vat_percentage = @vat, stock = @stock, alcoholic = @alcoholic WHERE drink_id = @id", connection);

                command.Parameters.AddWithValue("@name", drink.Name);
                command.Parameters.AddWithValue("@price", drink.Price);
                command.Parameters.AddWithValue("@vat", drink.VatPercentage);
                command.Parameters.AddWithValue("@stock", drink.Stock);
                command.Parameters.AddWithValue("@alcoholic", drink.Alcoholic);
                command.Parameters.AddWithValue("@id", drink.DrinkId);

                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating drink.", ex);
            }
        }

        public void Delete(int drinkId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(connectionString);
                using SqlCommand command = new SqlCommand(
                    "DELETE FROM dbo.Drink WHERE drink_id = @id", connection);
                command.Parameters.AddWithValue("@id", drinkId);

                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting drink with ID {drinkId}.", ex);
            }
        }
    }
}
