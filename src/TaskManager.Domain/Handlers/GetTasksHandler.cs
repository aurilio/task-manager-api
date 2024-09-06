using MediatR;
using System.Text.Json;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Repositories;
using TaskManager.Shareable.Requests;
using TaskManager.Shareable.Responses;

namespace TaskManager.Domain.Handlers
{
    internal class GetTasksHandler : IRequestHandler<GetTaskRequest, GetTaskResponse>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ICacheService _cacheService;

        public GetTasksHandler( ITaskRepository taskRepository, ICacheService cacheService)
        {
            _taskRepository = taskRepository;
            _cacheService = cacheService;
        }

        public async Task<GetTaskResponse> Handle(GetTaskRequest request, CancellationToken cancellationToken)
        {
            var dataFromCache = await _cacheService.GetCacheAsync(request.TaskId.ToString());

            if (!string.IsNullOrEmpty(dataFromCache))
            {
                var taskEntity = JsonSerializer.Deserialize<GetTaskResponse>(dataFromCache);
                return taskEntity;
            }

            var taskEntityFromDataBase = _taskRepository.GetTaskByIdAsync(request.TaskId);

            if (taskEntityFromDataBase.Result == null)
            {
                return new GetTaskResponse();
            }

            var taskJson = JsonSerializer.Serialize(taskEntityFromDataBase);
            await _cacheService.SetCacheAsync(taskEntityFromDataBase.Id.ToString(), taskJson);


            return new GetTaskResponse 
            {
                Id = taskEntityFromDataBase.Result.Id,
                Title = taskEntityFromDataBase.Result.Title,
                Description = taskEntityFromDataBase.Result.Description,
                IsCompleted = taskEntityFromDataBase.Result.IsCompleted,
                CreatedAt = taskEntityFromDataBase.Result.CreatedAt,
                UpdatedAt = taskEntityFromDataBase.Result.UpdatedAt,
            };
        }
    }
}