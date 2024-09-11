using MediatR;
using TaskManager.Shareable.DTOs;
using TaskManager.Shareable.Responses;

namespace TaskManager.Shareable.Requests;

public class CreateTaskRequest : IRequest<CreateTaskResponse>
{
    public TaskEntityCreateDTO TaskEntityDTO { get; set; } = default!;
}