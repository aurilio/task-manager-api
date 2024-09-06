using MediatR;
using StackExchange.Redis;
using System.Text.Json;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Repositories;
using TaskManager.Shareable.Requests;
using TaskManager.Shareable.Responses;

namespace TaskManager.Domain.Handlers
{
    public class CreateTaskHandler : IRequestHandler<CreateTaskRequest, CreateTaskResponse>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ICacheService _cacheService;
        private readonly IMessageBus _messageBus;

        public CreateTaskHandler(ITaskRepository taskRepository, ICacheService cacheService, IMessageBus messageBus)
        {
            _taskRepository = taskRepository;
            _cacheService = cacheService;
            _messageBus = messageBus;
        }

        public async Task<CreateTaskResponse> Handle(CreateTaskRequest request, CancellationToken cancellationToken)
        {
            var taskEntity = new TaskEntity
            {
                Id = Guid.NewGuid(),
                Title = request.TaskEntityDTO.Title,
                Description = request.TaskEntityDTO.Description,
                IsCompleted = request.TaskEntityDTO.IsCompleted,
                CreatedAt = DateTime.UtcNow
            };

            await _taskRepository.AddTaskAsync(taskEntity);
            
            var taskJson = JsonSerializer.Serialize(taskEntity);
            
            await _cacheService.SetCacheAsync(taskEntity.Id.ToString(), taskJson);

            _messageBus.Publish("taskQueue", taskJson);

            return new CreateTaskResponse { TaskId = taskEntity.Id };
        }
    }
}