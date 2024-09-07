using MediatR;
using TaskManager.Shareable.Responses;

namespace TaskManager.Shareable.Requests
{
    public class DeleteTaskRequest : IRequest<DeleteTaskResponse>
    {
        public Guid TaskId { get; set; }
    }
}
