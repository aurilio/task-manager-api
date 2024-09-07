using MediatR;
using TaskManager.Shareable.Responses;

namespace TaskManager.Shareable.Requests
{
    public class GetTaskRequest : IRequest<GetTaskResponse>
    {
        public Guid TaskId { get; set; }
    }
}