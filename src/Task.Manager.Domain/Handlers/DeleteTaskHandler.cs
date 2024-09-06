using MediatR;
using Task.Manager.Domain.Repositories;
using Task.Manager.Shareable.Requests;
using Task.Manager.Shareable.Responses;

namespace Task.Manager.Domain.Handlers
{
    public class DeleteTaskHandler : IRequestHandler<DeleteTaskRequest, DeleteTaskResponse>
    {
        private readonly ITaskRepository _taskRepository;

        public DeleteTaskHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<DeleteTaskResponse> Handle(DeleteTaskRequest request, CancellationToken cancellationToken)
        {
            _taskRepository.DeleteTaskAsync(request.TaskId);

            return new DeleteTaskResponse() { Success = false, Message = "Tarefa excluída com sucesso." };
        }
    }
}