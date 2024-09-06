using TaskManager.Shareable.DTOs;

namespace TaskManager.Shareable.Responses
{
    public class GetTasksBySearchResponse
    {
        public List<TaskEntityDTO> Tasks { get; set; } = new List<TaskEntityDTO>();
    }
}
