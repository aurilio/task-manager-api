using Task.Manager.Shareable.DTOs;

namespace Task.Manager.Shareable.Responses
{
    public class GetAllTaskResponse
    {
        public List<TaskEntityDTO> Tasks { get; set; }
    }
}
