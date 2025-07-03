using Core.Domain;

namespace Core.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.Courses.Any())
            {
                // Instructores
                var instructor1 = new Instructor("Juan Pérez");
                var instructor2 = new Instructor("Ana Gómez");

                // Curso
                var course = new Course("Matemáticas", "Curso de matemáticas básicas");
                course.AddInstructor(instructor1);
                course.AddInstructor(instructor2);

                // Módulos
                var module1 = new Module("Álgebra");
                var module2 = new Module("Geometría");

                // Lecciones
                var lesson1 = new Lesson("Ecuaciones", "Contenido de ecuaciones");
                var lesson2 = new Lesson("Polígonos", "Contenido de polígonos");

                module1.AddLesson(lesson1);
                module2.AddLesson(lesson2);

                course.AddModule(module1);
                course.AddModule(module2);

                context.Courses.Add(course);
                context.SaveChanges();
            }
        }
    }
} 