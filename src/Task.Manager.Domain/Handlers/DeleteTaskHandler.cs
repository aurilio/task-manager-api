using MediatR;
using Task.Manager.Domain.Interfaces;
using Task.Manager.Domain.Repositories;
using Task.Manager.Shareable.Requests;
using Task.Manager.Shareable.Responses;

namespace Task.Manager.Domain.Handlers
{
    public class DeleteTaskHandler : IRequestHandler<DeleteTaskRequest, DeleteTaskResponse>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ICacheService _cacheService;
        private readonly IMessageBus _messageBus;

        public DeleteTaskHandler(ITaskRepository taskRepository, ICacheService cacheService, IMessageBus messageBus)
        {
            _taskRepository = taskRepository;
            _cacheService = cacheService;
            _messageBus = messageBus;
        }

        public async Task<DeleteTaskResponse> Handle(DeleteTaskRequest request, CancellationToken cancellationToken)
        {
            _taskRepository.DeleteTaskAsync(request.TaskId);
            
            _cacheService.RemoveCacheAsync(request.TaskId.ToString());

            return new DeleteTaskResponse() { Success = false, Message = "Tarefa excluída com sucesso." };
        }
    }
}