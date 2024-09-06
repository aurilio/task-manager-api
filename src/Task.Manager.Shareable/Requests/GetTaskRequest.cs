using MediatR;
using Task.Manager.Shareable.Responses;

namespace Task.Manager.Shareable.Requests
{
    public class GetTaskRequest : IRequest<GetTaskResponse>
    {
        public Guid TaskId { get; set; }
    }
}