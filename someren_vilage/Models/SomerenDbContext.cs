using Microsoft.EntityFrameworkCore;

namespace someren_vilage.Models
{
    public class SomerenDbContext : DbContext
    {
        public SomerenDbContext(DbContextOptions<SomerenDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityParticipant> ActivityParticipants { get; set; }
        public DbSet<Drink> Drinks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite key for ActivityParticipant
            modelBuilder.Entity<ActivityParticipant>()
                .HasKey(ap => new { ap.StudentId, ap.ActivityId });

            // Composite key for OrderItem
            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new { oi.OrderId, oi.DrinkId });

            base.OnModelCreating(modelBuilder);
        }
    }
}