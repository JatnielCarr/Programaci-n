namespace Core.Domain
{
    public class Module
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public List<Lesson> Lessons { get; private set; }
        public Guid? InstructorId { get; private set; }
        public Instructor Instructor { get; private set; }
        public bool IsPublished { get; private set; }

        public Module(string title)
        {
            Id = Guid.NewGuid();
            Title = title;
            Lessons = new List<Lesson>();
            IsPublished = false;
        }

        public Module() { }

        public void AddLesson(Lesson lesson)
        {
            Lessons.Add(lesson);
        }

        public void RemoveLesson(Guid lessonId)
        {
            var lesson = Lessons.FirstOrDefault(l => l.Id == lessonId);
            if (lesson != null)
                Lessons.Remove(lesson);
        }

        public void SetTitle(string title) => Title = title;

        public void AssignInstructor(Instructor instructor)
        {
            if (IsPublished) throw new InvalidOperationException("No se puede asignar instructor a un módulo publicado.");
            Instructor = instructor;
            InstructorId = instructor.Id;
        }

        public void RemoveInstructor()
        {
            if (IsPublished) throw new InvalidOperationException("No se puede eliminar el instructor de un módulo publicado.");
            Instructor = null;
            InstructorId = null;
        }

        public void Publish()
        {
            if (Instructor == null) throw new InvalidOperationException("No se puede publicar un módulo sin instructor asignado.");
            IsPublished = true;
        }
    }
} 