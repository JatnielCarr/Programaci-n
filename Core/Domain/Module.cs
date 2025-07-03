namespace Core.Domain
{
    public class Module
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public List<Lesson> Lessons { get; private set; }

        public Module(string title)
        {
            Id = Guid.NewGuid();
            Title = title;
            Lessons = new List<Lesson>();
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
    }
} 