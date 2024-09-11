using TaskManager.Shareable.DTOs;

namespace TaskManager.Shareable.Responses;

public class GetTaskResponse
{
    public TaskEntityGetDTO TaskEntityGetDto { get; set; } = new TaskEntityGetDTO();
}