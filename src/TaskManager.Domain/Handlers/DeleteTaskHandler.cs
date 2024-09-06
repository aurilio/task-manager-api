using MediatR;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Repositories;
using TaskManager.Shareable.Requests;
using TaskManager.Shareable.Responses;

namespace TaskManager.Domain.Handlers
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
            await _taskRepository.DeleteTaskAsync(request.TaskId);
            
            await _cacheService.RemoveCacheAsync(request.TaskId.ToString());

            return new DeleteTaskResponse() { Success = false, Message = "Tarefa excluída com sucesso." };
        }
    }
}