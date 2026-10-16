
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Models;

namespace RaceDayAPI.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(
        DbContextOptions<RaceDayDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<RaceDayAPI.Models.Route> Routes { get; set; }
        public DbSet<Weather> Weather { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }

        protected override void OnModelCreating(
        ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users", "dbo");
            modelBuilder.Entity<Event>().ToTable("Events", "dbo");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollments", "dbo");
            modelBuilder.Entity<Result>().ToTable("Results", "dbo");
            modelBuilder.Entity<RaceDayAPI.Models.Route>().ToTable("Routes", "dbo");
            modelBuilder.Entity<Weather>().ToTable("Weather", "dbo");
            modelBuilder.Entity<Category>().ToTable("Categories", "dbo");
            modelBuilder.Entity<EventCategory>().ToTable("EventCategory", "dbo");
            modelBuilder.Entity<EventCategory>()
   .HasKey(ec => new { ec.EventID, ec.CategoryID });

            modelBuilder.Entity<Category>()
            .Property(c => c.ActivityType)
            .HasColumnName("ActivityType");
        }
    }
}
