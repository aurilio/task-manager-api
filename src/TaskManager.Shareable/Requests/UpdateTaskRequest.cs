using MediatR;
using TaskManager.Shareable.DTOs;
using TaskManager.Shareable.Responses;

namespace TaskManager.Shareable.Requests;

public class UpdateTaskRequest : IRequest<UpdateTaskResponse>
{
    public TaskEntityUpdateDTO TaskEntityDTO { get; set; } = default!;
}