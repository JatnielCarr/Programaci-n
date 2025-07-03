using Microsoft.EntityFrameworkCore;
using Core.Domain;

namespace Core.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Instructor> Instructors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Course
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Modules)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Instructors)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            // Module
            modelBuilder.Entity<Module>()
                .HasMany(m => m.Lessons)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            // Instructor: Name unique per course (handled in logic)
        }
    }
} 