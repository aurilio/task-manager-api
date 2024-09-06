using MediatR;
using Serilog;
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
        private readonly IElasticSearchRepository _elasticSearchRepository;
        private readonly ILogger _logger;

        public UpdateTaskHandler(
            ITaskRepository taskRepository,
            ICacheService cacheService,
            IMessageBus messageBus,
            IElasticSearchRepository elasticSearchRepository,
            ILogger logger)
        {
            _taskRepository = taskRepository;
            _cacheService = cacheService;
            _messageBus = messageBus;
            _elasticSearchRepository = elasticSearchRepository;
            _logger = logger;
        }

        public async Task<UpdateTaskResponse> Handle(UpdateTaskRequest request, CancellationToken cancellationToken)
        {
            _logger.Information("[UpdateTaskHandler] Iniciando autualização de dados Tarefa com TaskId: {TaskId}", request.TaskEntityDTO.Id);

            var taskEntity = await _taskRepository.GetTaskByIdAsync(request.TaskEntityDTO.Id);

            if (taskEntity == null)
            {
                _logger.Warning("[UpdateTaskHandler] Tarefa com Id: {TaskId} não encontrada", request.TaskEntityDTO.Id);
                return new UpdateTaskResponse { Success = false, Message = "Tarefa não encontrada." };
            }

            _logger.Information("[UpdateTaskHandler] Atualizando tarefa com Id: {TaskId}", taskEntity.Id);

            taskEntity.Title = request.TaskEntityDTO.Title;
            taskEntity.Description = request.TaskEntityDTO.Description;
            taskEntity.IsCompleted = request.TaskEntityDTO.IsCompleted;
            taskEntity.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _taskRepository.UpdateTaskAsync(taskEntity);
                _logger.Information("[UpdateTaskHandler] Tarefa com TaskId: {TaskId} atualizado no banco de dados", taskEntity.Id);

                await _elasticSearchRepository.SetTaskAsync(taskEntity);
                _logger.Information("[UpdateTaskHandler] Tarefa com TaskId: {TaskId} incluido no ElasticSearch", taskEntity.Id);

                var taskJson = JsonSerializer.Serialize(taskEntity);

                await _cacheService.SetCacheAsync(taskEntity.Id.ToString(), taskJson);
                _logger.Information("[UpdateTaskHandler] Tarefa com TaskId: {TaskId} inserido no cache Redis", taskEntity.Id);

                _messageBus.Publish("taskQueue", taskJson);

                _logger.Information("[UpdateTaskHandler] Tarefa com TaskId: {TaskId} publicada no RabbitMQ", taskEntity.Id);

                return new UpdateTaskResponse { Success = true, Message = "Tarefa atualizada com sucesso!" };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[UpdateTaskHandler] Erro ao atualizar tarefa com TaskId: {TaskId}", taskEntity.Id);
                throw;
            }
        }
    }
}