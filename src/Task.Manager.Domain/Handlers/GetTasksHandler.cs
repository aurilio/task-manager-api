using MediatR;
using System.Text.Json;
using Task.Manager.Domain.Interfaces;
using Task.Manager.Domain.Repositories;
using Task.Manager.Shareable.Requests;
using Task.Manager.Shareable.Responses;

namespace Task.Manager.Domain.Handlers
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
                // Deserializa os dados do cache e retorna a resposta
                var taskEntity = JsonSerializer.Deserialize<GetTaskResponse>(dataFromCache);
                return taskEntity;
            }

            var taskEntityFromDataBase = _taskRepository.GetTaskByIdAsync(request.TaskId);

            if (taskEntityFromDataBase.Result == null)
            {
                return new GetTaskResponse();
            }

            var taskJson = JsonSerializer.Serialize(taskEntityFromDataBase);
            _cacheService.SetCacheAsync(taskEntityFromDataBase.Id.ToString(), taskJson);


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