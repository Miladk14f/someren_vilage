using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using someren_vilage.Models;

namespace someren_vilage.Controllers
{
    public class SetupController : Controller
    {
        private readonly IConfiguration _configuration;

        public SetupController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new DatabaseSetupModel();

            // Pre-fill from existing connection string if available
            string? existing = _configuration.GetConnectionString("SomerenDb");
            if (!string.IsNullOrWhiteSpace(existing))
            {
                model.ConnectionString = existing;

                try
                {
                    var builder = new SqlConnectionStringBuilder(existing);
                    string dataSource = builder.DataSource ?? string.Empty;

                    // Parse server and port from "tcp:server,port"
                    string server = dataSource;
                    int port = 1433;

                    if (server.StartsWith("tcp:", StringComparison.OrdinalIgnoreCase))
                        server = server[4..];

                    if (server.Contains(','))
                    {
                        string[] parts = server.Split(',');
                        server = parts[0];
                        if (int.TryParse(parts[1], out int parsedPort))
                            port = parsedPort;
                    }

                    model.Server = server;
                    model.Port = port;
                    model.Database = builder.InitialCatalog;
                    model.UserId = builder.UserID;
                    model.Encrypt = builder.Encrypt == SqlConnectionEncryptOption.Mandatory ? "Mandatory" : "Optional";
                    model.TrustServerCertificate = builder.TrustServerCertificate;
                }
                catch
                {
                    // Ignore parse errors, show empty form
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(DatabaseSetupModel model)
        {
            string connectionString;

            if (model.InputMode == "connectionstring")
            {
                // User pasted a full connection string
                if (string.IsNullOrWhiteSpace(model.ConnectionString))
                {
                    ModelState.AddModelError(nameof(model.ConnectionString), "Connection string is required.");
                    return View(model);
                }

                connectionString = model.ConnectionString.Trim();
            }
            else
            {
                // User filled in individual properties
                if (string.IsNullOrWhiteSpace(model.Server))
                    ModelState.AddModelError(nameof(model.Server), "Server name is required.");
                if (string.IsNullOrWhiteSpace(model.Database))
                    ModelState.AddModelError(nameof(model.Database), "Database name is required.");
                if (string.IsNullOrWhiteSpace(model.UserId))
                    ModelState.AddModelError(nameof(model.UserId), "User name is required.");
                if (string.IsNullOrWhiteSpace(model.Password))
                    ModelState.AddModelError(nameof(model.Password), "Password is required.");

                if (!ModelState.IsValid)
                    return View(model);

                connectionString = model.BuildConnectionString();
            }

            // Test the connection before saving
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Could not connect to database: {ex.Message}");
                return View(model);
            }

            // Update the in-memory configuration
            _configuration["ConnectionStrings:SomerenDb"] = connectionString;

            TempData["Success"] = "Database connection configured successfully!";
            return RedirectToAction("Index", "Home");
        }
    }
}
