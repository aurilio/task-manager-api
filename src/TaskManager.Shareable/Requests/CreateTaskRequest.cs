using MediatR;
using TaskManager.Shareable.DTOs;
using TaskManager.Shareable.Responses;

namespace TaskManager.Shareable.Requests
{
    public class CreateTaskRequest : IRequest<CreateTaskResponse>
    {
        public TaskEntityDTO TaskEntityDTO { get; set; } = default!;
    }
}