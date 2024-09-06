using MediatR;
using System.Text.Json;
using Task.Manager.Domain.Interfaces;
using Task.Manager.Domain.Repositories;
using Task.Manager.Shareable.Requests;
using Task.Manager.Shareable.Responses;

namespace Task.Manager.Domain.Handlers
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

            _taskRepository.UpdateTaskAsync(taskEntity);

            var taskJson = JsonSerializer.Serialize(taskEntity);

            _cacheService.SetCacheAsync(taskEntity.Id.ToString(), taskJson);

            _messageBus.Publish("taskQueue", taskJson);

            return new UpdateTaskResponse { Success = true, Message = "Tarefa atualizada com sucesso!" };
        }
    }
}