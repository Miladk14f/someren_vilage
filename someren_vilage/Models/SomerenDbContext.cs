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
        public DbSet<LecturerActivity> LecturerActivities { get; set; }
        public DbSet<Drink> Drinks { get; set; }
        public DbSet<Order> Orders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite key for ActivityParticipant (student_number, activity_id)
            modelBuilder.Entity<ActivityParticipant>()
                .HasKey(ap => new { ap.StudentNumber, ap.ActivityId });

            // Composite key for LecturerActivity (lecturer_id, activity_id)
            modelBuilder.Entity<LecturerActivity>()
                .HasKey(la => new { la.LecturerId, la.ActivityId });

            // Lecturer -> Room unique constraint (one Lecturer per Room)
            modelBuilder.Entity<Lecturer>()
                .HasIndex(l => l.RoomId)
                .IsUnique();

            // Configure Student primary key (student_number)
            modelBuilder.Entity<Student>()
                .HasKey(s => s.StudentNumber);

            // Configure Room primary key
            modelBuilder.Entity<Room>()
                .HasKey(r => r.RoomId);

            // Configure Activity primary key
            modelBuilder.Entity<Activity>()
                .HasKey(a => a.ActivityId);

            // Configure Drink primary key
            modelBuilder.Entity<Drink>()
                .HasKey(d => d.DrinkId);

            // Configure Order primary key
            modelBuilder.Entity<Order>()
                .HasKey(o => o.OrderId);

            base.OnModelCreating(modelBuilder);
        }
    }
}