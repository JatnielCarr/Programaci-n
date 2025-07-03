namespace Core.Domain
{
    public class Lesson
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }

        public Lesson(string title, string content)
        {
            Id = Guid.NewGuid();
            Title = title;
            Content = content;
        }

        public Lesson() { }

        public void SetTitle(string title) => Title = title;
        public void SetContent(string content) => Content = content;
    }
} 