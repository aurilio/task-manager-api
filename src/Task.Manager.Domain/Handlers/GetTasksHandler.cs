using MediatR;
using Task.Manager.Shareable.Requests;
using Task.Manager.Shareable.Responses;

namespace Task.Manager.Domain.Handlers
{
    internal class GetTasksHandler : IRequestHandler<GetTaskRequest, GetTaskResponse>
    {
        public async Task<GetTaskResponse> Handle(GetTaskRequest request, CancellationToken cancellationToken)
        {
            return new GetTaskResponse();
        }
    }
}