using TaskManager.Shareable.DTOs;

namespace TaskManager.Shareable.Responses
{
    public class GetAllTaskResponse
    {
        public List<TaskEntityDTO> Tasks { get; set; } = new List<TaskEntityDTO>();
        
        public int TotalPages { get; set; }
        
        public int CurrentPage { get; set; }
    }
}