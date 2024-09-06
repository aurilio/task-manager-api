using MediatR;
using Task.Manager.Shareable.Requests;
using Task.Manager.Shareable.Responses;

namespace Task.Manager.Domain.Handlers
{
    public class DeleteTaskHandler : IRequestHandler<DeleteTaskRequest, DeleteTaskResponse>
    {
        public async Task<DeleteTaskResponse> Handle(DeleteTaskRequest request, CancellationToken cancellationToken)
        {
            return new DeleteTaskResponse() { Success = true };
        }
    }
}