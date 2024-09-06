using MediatR;
using System.Text.Json;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Repositories;
using TaskManager.Shareable.Requests;
using TaskManager.Shareable.Responses;

namespace TaskManager.Domain.Handlers
{
    public class UpdateTaskHandler : IRequestHandler<UpdateTaskRequest, UpdateTaskResponse>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ICacheService _cacheService;
        private readonly IMessageBus _messageBus;

        public UpdateTaskHandler(ITaskRepository taskRepository, ICacheService cacheService, IMessageBus messageBus)
        {
            _taskRepository = taskRepository;
            _cacheService = cacheService;
            _messageBus = messageBus;
        }

        public async Task<UpdateTaskResponse> Handle(UpdateTaskRequest request, CancellationToken cancellationToken)
        {
            var taskEntity = await _taskRepository.GetTaskByIdAsync(request.TaskEntityDTO.Id);

            if (taskEntity == null)
            {
                return new UpdateTaskResponse { Success = false, Message = "Tarefa não encontrada." };
            }

            taskEntity.Title = request.TaskEntityDTO.Title;
            taskEntity.Description = request.TaskEntityDTO.Description;
            taskEntity.IsCompleted = request.TaskEntityDTO.IsCompleted;
            taskEntity.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateTaskAsync(taskEntity);

            var taskJson = JsonSerializer.Serialize(taskEntity);

            await _cacheService.SetCacheAsync(taskEntity.Id.ToString(), taskJson);

            _messageBus.Publish("taskQueue", taskJson);

            return new UpdateTaskResponse { Success = true, Message = "Tarefa atualizada com sucesso!" };
        }
    }
}