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
        public DbSet<CourseInstructor> CourseInstructors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Nombres de tablas en minúsculas para PostgreSQL
            modelBuilder.Entity<Course>().ToTable("courses");
            modelBuilder.Entity<Module>().ToTable("modules");
            modelBuilder.Entity<Lesson>().ToTable("lessons");
            modelBuilder.Entity<Instructor>().ToTable("instructors");
            modelBuilder.Entity<CourseInstructor>().ToTable("course_instructors");

            // Course
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Modules)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            // Relación muchos a muchos entre Course e Instructor
            modelBuilder.Entity<CourseInstructor>()
                .HasKey(ci => new { ci.CourseId, ci.InstructorId });
            modelBuilder.Entity<CourseInstructor>()
                .HasOne(ci => ci.Course)
                .WithMany(c => c.CourseInstructors)
                .HasForeignKey(ci => ci.CourseId);
            modelBuilder.Entity<CourseInstructor>()
                .HasOne(ci => ci.Instructor)
                .WithMany()
                .HasForeignKey(ci => ci.InstructorId);

            // Module
            modelBuilder.Entity<Module>()
                .HasMany(m => m.Lessons)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            // Instructor: Name unique per course (handled in logic)
        }
    }
} 