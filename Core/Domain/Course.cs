namespace Core.Domain
{
    public class Course
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public bool IsPublished { get; private set; }
        public List<Module> Modules { get; private set; }
        public List<Instructor> Instructors { get; private set; }

        public Course() { }

        public Course(string title, string description)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            IsPublished = false;
            Modules = new List<Module>();
            Instructors = new List<Instructor>();
        }

        public void AddModule(Module module)
        {
            if (IsPublished) throw new InvalidOperationException("No se pueden agregar módulos a un curso publicado.");
            Modules.Add(module);
        }

        public void Publish()
        {
            IsPublished = true;
        }

        public void AddInstructor(Instructor instructor)
        {
            if (Instructors.Any(i => i.Name == instructor.Name))
                throw new InvalidOperationException("No se pueden agregar instructores con nombre repetido.");
            Instructors.Add(instructor);
        }

        public void RemoveInstructor(Guid instructorId)
        {
            if (IsPublished) throw new InvalidOperationException("No se pueden eliminar instructores de un curso publicado.");
            var instructor = Instructors.FirstOrDefault(i => i.Id == instructorId);
            if (instructor != null)
                Instructors.Remove(instructor);
        }

        public void SetTitle(string title) => Title = title;
        public void SetDescription(string description) => Description = description;
    }
} 