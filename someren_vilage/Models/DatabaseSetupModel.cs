using System.ComponentModel.DataAnnotations;

namespace someren_vilage.Models
{
    public class DatabaseSetupModel
    {
        // "properties" or "connectionstring"
        [Display(Name = "Input Mode")]
        public string InputMode { get; set; } = "properties";

        // --- Connection Properties tab ---

        [Display(Name = "Server Name")]
        public string Server { get; set; } = string.Empty;

        [Display(Name = "Port")]
        public int Port { get; set; } = 1433;

        [Display(Name = "Database Name")]
        public string Database { get; set; } = string.Empty;

        [Display(Name = "User Name")]
        public string UserId { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Encrypt")]
        public string Encrypt { get; set; } = "Mandatory";

        [Display(Name = "Trust Server Certificate")]
        public bool TrustServerCertificate { get; set; } = false;

        // --- Connection String tab ---

        [Display(Name = "Connection String")]
        public string ConnectionString { get; set; } = string.Empty;

        public string BuildConnectionString()
        {
            return $"Data Source=tcp:{Server},{Port};" +
                   $"Initial Catalog={Database};" +
                   $"Persist Security Info=False;" +
                   $"User ID={UserId};" +
                   $"Password={Password};" +
                   $"Pooling=False;" +
                   $"MultipleActiveResultSets=False;" +
                   $"Connect Timeout=30;" +
                   $"Encrypt={Encrypt};" +
                   $"TrustServerCertificate={TrustServerCertificate}";
        }
    }
}
