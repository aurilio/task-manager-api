using MediatR;
using Serilog;
using System.Text.Json;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Repositories;
using TaskManager.Shareable.Requests;
using TaskManager.Shareable.Responses;

namespace TaskManager.Domain.Handlers;

public class CreateTaskHandler : IRequestHandler<CreateTaskRequest, CreateTaskResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICacheService _cacheService;
    private readonly IMessageBus _messageBus;
    private readonly IElasticSearchRepository _elasticSearchRepository;
    private readonly ILogger _logger;

    public CreateTaskHandler(
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

    public async Task<CreateTaskResponse> Handle(CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var taskId = Guid.NewGuid();
        _logger.Information("[CreateTaskHandler] Iniciando a Inclusão da tarefa com TaskId: {TaskId}", taskId);

        var taskEntity = new TaskEntity
        {
            Id = taskId,
            Title = request.TaskEntityDTO.Title,
            Description = request.TaskEntityDTO.Description,
            IsCompleted = request.TaskEntityDTO.IsCompleted,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            _logger.Information("[CreateTaskHandler] Criando tarefa com TaskId: {TaskId}", taskEntity.Id);
            await _taskRepository.AddTaskAsync(taskEntity);

            _logger.Information("[CreateTaskHandler] Incluindo registro no ElasticSearch com TaskId: {TaskId}", taskEntity.Id);
            await _elasticSearchRepository.SetTaskAsync(taskEntity);

            var taskJson = JsonSerializer.Serialize(taskEntity);

            _logger.Information("[CreateTaskHandler] Incluindo tarefa no cache do Redis com TaskId: {TaskId}", taskEntity.Id);
            await _cacheService.SetCacheAsync(taskEntity.Id.ToString(), taskJson);

            _logger.Information("[CreateTaskHandler] Publicação de tarefa no Rabbitmq com TaskId: {TaskId}", taskEntity.Id);
            _messageBus.Publish("taskQueue", taskJson);

            _logger.Information("[CreateTaskHandler] Tarefa com TaskId: {TaskId} criada com sucesso", taskEntity.Id);

            return new CreateTaskResponse { TaskId = taskEntity.Id };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[CreateTaskHandler] Erro ao criar tarefa com TaskId: {TaskId}", taskEntity.Id);
            throw;
        }
    }
}