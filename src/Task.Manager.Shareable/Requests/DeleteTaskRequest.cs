using MediatR;
using Task.Manager.Shareable.Responses;

namespace Task.Manager.Shareable.Requests
{
    public class DeleteTaskRequest : IRequest<DeleteTaskResponse>
    {
        public Guid TaskId { get; set; }
    }
}
