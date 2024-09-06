using MediatR;
using Task.Manager.Shareable.DTOs;
using Task.Manager.Shareable.Responses;

namespace Task.Manager.Shareable.Requests
{
    public class UpdateTaskRequest : IRequest<UpdateTaskResponse>
    {
        public TaskEntityDTO TaskEntityDTO { get; set; }
    }
}