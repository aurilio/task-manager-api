namespace Task.Manager.Domain.Entities
{
    public class TaskEntity
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = default!;

        public string Description { get; set; } = default!;

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }
    }
}
