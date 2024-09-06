using MediatR;
using Task.Manager.Shareable.DTOs;
using Task.Manager.Shareable.Requests;
using Task.Manager.Shareable.Responses;

namespace Task.Manager.Domain.Handlers
{
    public class GetAllTasksHandler : IRequestHandler<GetAllTasksRequest, GetAllTaskResponse>
    {

        public async Task<GetAllTaskResponse> Handle(GetAllTasksRequest request, CancellationToken cancellationToken)
        {
            return new GetAllTaskResponse
            {
                Tasks = new List<TaskEntityDTO>() // Aqui você pode preencher com tarefas reais
            };
        }
    }
}