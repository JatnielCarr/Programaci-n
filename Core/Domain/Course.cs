namespace Core.Domain
{
    public class Course
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public bool IsPublished { get; private set; }
        public List<Module> Modules { get; private set; }
        public List<CourseInstructor> CourseInstructors { get; private set; }

        public Course() { }

        public Course(string title, string description)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            IsPublished = false;
            Modules = new List<Module>();
            CourseInstructors = new List<CourseInstructor>();
        }

        public void AddModule(Module module)
        {
            if (IsPublished) throw new InvalidOperationException("No se pueden agregar módulos a un curso publicado.");
            Modules.Add(module);
        }

        public void UpdateModule(Guid moduleId, string newTitle)
        {
            if (IsPublished) throw new InvalidOperationException("No se pueden modificar módulos de un curso publicado.");
            var module = Modules.FirstOrDefault(m => m.Id == moduleId);
            if (module != null)
                module.SetTitle(newTitle);
        }

        public void RemoveModule(Guid moduleId)
        {
            if (IsPublished) throw new InvalidOperationException("No se pueden eliminar módulos de un curso publicado.");
            var module = Modules.FirstOrDefault(m => m.Id == moduleId);
            if (module != null)
                Modules.Remove(module);
        }

        public void AddLessonToModule(Guid moduleId, Lesson lesson)
        {
            if (IsPublished) throw new InvalidOperationException("No se pueden agregar lecciones a un curso publicado.");
            var module = Modules.FirstOrDefault(m => m.Id == moduleId);
            if (module != null)
                module.AddLesson(lesson);
        }

        public void UpdateLesson(Guid moduleId, Guid lessonId, string newTitle, string newContent)
        {
            if (IsPublished) throw new InvalidOperationException("No se pueden modificar lecciones de un curso publicado.");
            var module = Modules.FirstOrDefault(m => m.Id == moduleId);
            var lesson = module?.Lessons.FirstOrDefault(l => l.Id == lessonId);
            if (lesson != null)
            {
                lesson.SetTitle(newTitle);
                lesson.SetContent(newContent);
            }
        }

        public void RemoveLesson(Guid moduleId, Guid lessonId)
        {
            if (IsPublished) throw new InvalidOperationException("No se pueden eliminar lecciones de un curso publicado.");
            var module = Modules.FirstOrDefault(m => m.Id == moduleId);
            if (module != null)
                module.RemoveLesson(lessonId);
        }

        public void Publish()
        {
            IsPublished = true;
        }

        public void AddInstructor(Instructor instructor)
        {
            if (CourseInstructors.Any(ci => ci.Instructor.Name == instructor.Name))
                throw new InvalidOperationException("No se pueden agregar instructores con nombre repetido.");
            CourseInstructors.Add(new CourseInstructor { CourseId = this.Id, InstructorId = instructor.Id, Instructor = instructor });
        }

        public void RemoveInstructor(Guid instructorId)
        {
            if (IsPublished) throw new InvalidOperationException("No se pueden eliminar instructores de un curso publicado.");
            var ci = CourseInstructors.FirstOrDefault(x => x.InstructorId == instructorId);
            if (ci != null)
                CourseInstructors.Remove(ci);
        }

        public void SetTitle(string title) => Title = title;
        public void SetDescription(string description) => Description = description;
    }
} 