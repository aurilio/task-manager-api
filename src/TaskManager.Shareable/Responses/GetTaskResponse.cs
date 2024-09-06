using TaskManager.Shareable.DTOs;

namespace TaskManager.Shareable.Responses
{
    public class GetTaskResponse
    {
        //public TaskEntityDTO TaskEntityRequest { get; set; }

        public Guid? Id { get; set; }

        public string Title { get; set; } = default!;

        public string Description { get; set; } = default!;

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }
    }
}