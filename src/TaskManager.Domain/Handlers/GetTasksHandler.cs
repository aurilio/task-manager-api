using MediatR;
using System.Text.Json;
using Serilog;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Repositories;
using TaskManager.Shareable.DTOs;
using TaskManager.Shareable.Requests;
using TaskManager.Shareable.Responses;

namespace TaskManager.Domain.Handlers
{
    internal class GetTasksHandler : IRequestHandler<GetTaskRequest, GetTaskResponse>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger _logger;

        public GetTasksHandler(
            ITaskRepository taskRepository,
            ICacheService cacheService,
            ILogger logger)
        {
            _taskRepository = taskRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<GetTaskResponse> Handle(GetTaskRequest request, CancellationToken cancellationToken)
        {
            _logger.Information("[GetTasksHandler] Iniciando busca da Tarefa com TaskId: {TaskId}: .", request.TaskId);

            var dataFromCache = await _cacheService.GetCacheAsync(request.TaskId.ToString());

            if (!string.IsNullOrEmpty(dataFromCache))
            {
                _logger.Information("[GetTasksHandler] Tarefa com TaskId: {TaskId} encontrada no cache Redis.", request.TaskId);
                var taskEntity = JsonSerializer.Deserialize<GetTaskResponse>(dataFromCache);
                return taskEntity;
            }

            _logger.Information("[GetTasksHandler] Tarefa com TaskId: {TaskId} não encontrada no cache Redis. Buscando no banco de dados.", request.TaskId);

            var taskEntityFromDataBase = _taskRepository.GetTaskByIdAsync(request.TaskId);
            if (taskEntityFromDataBase.Result == null)
            {
                _logger.Warning("[GetTasksHandler] Tarefa com TaskId: {TaskId} não encontrada no banco de dados.", request.TaskId);
                return new GetTaskResponse();
            }

            _logger.Information("[GetTasksHandler] Salvando tarefa com TaskId: {TaskId} no cache Redis.", request.TaskId);
            var taskJson = JsonSerializer.Serialize(taskEntityFromDataBase);
            await _cacheService.SetCacheAsync(taskEntityFromDataBase.Id.ToString(), taskJson);

            var taskDto = new TaskEntityDTO
            {
                Id = taskEntityFromDataBase.Result.Id,
                Title = taskEntityFromDataBase.Result.Title,
                Description = taskEntityFromDataBase.Result.Description,
                IsCompleted = taskEntityFromDataBase.Result.IsCompleted,
                CreatedAt = taskEntityFromDataBase.Result.CreatedAt,
                UpdatedAt = taskEntityFromDataBase.Result.UpdatedAt
            };

            _logger.Information("[GetTasksHandler] Busca da tarefa com TaskId: {TaskId} concluída.", request.TaskId);

            return new GetTaskResponse { TaskEntityDto = taskDto }; ;
        }
    }
}