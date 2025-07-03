namespace Core.Domain
{
    public class Instructor
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public Instructor(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        public Instructor() { }

        public void SetName(string name) => Name = name;
    }
} 