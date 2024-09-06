using MediatR;
using Task.Manager.Shareable.DTOs;
using Task.Manager.Shareable.Responses;

namespace Task.Manager.Shareable.Requests
{
    public class CreateTaskRequest : IRequest<CreateTaskResponse>
    {
        public TaskEntityDTO TaskEntityDTO { get; set; }
    }
}