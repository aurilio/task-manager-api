using TaskManager.Shareable.DTOs;

namespace TaskManager.Shareable.Responses
{
    public class GetAllTaskResponse
    {
        public List<TaskEntityDTO> Tasks { get; set; }
    }
}
