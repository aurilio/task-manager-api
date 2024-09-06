using MediatR;
using Task.Manager.Shareable.Requests;
using Task.Manager.Shareable.Responses;

namespace Task.Manager.Domain.Handlers
{
    public class UpdateTaskHandler : IRequestHandler<UpdateTaskRequest, UpdateTaskResponse>
    {
        public async Task<UpdateTaskResponse> Handle(UpdateTaskRequest request, CancellationToken cancellationToken)
        {
            return new UpdateTaskResponse() { Success = true };
        }
    }
}