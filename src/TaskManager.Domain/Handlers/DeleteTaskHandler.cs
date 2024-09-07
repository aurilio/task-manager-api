using MediatR;
using Serilog;
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
        private readonly IElasticSearchRepository _elasticSearchRepository;
        private readonly ILogger _logger;

        public DeleteTaskHandler(
            ITaskRepository taskRepository,
            ICacheService cacheService,
            IElasticSearchRepository elasticSearchRepository,
            ILogger logger)
        {
            _taskRepository = taskRepository;
            _cacheService = cacheService;
            _elasticSearchRepository = elasticSearchRepository;
            _logger = logger;
        }

        public async Task<DeleteTaskResponse> Handle(DeleteTaskRequest request, CancellationToken cancellationToken)
        {
            _logger.Information("[DeleteTaskHandler] Iniciando exclusão de dados da Tarefa com TaskId: {TaskId}", request.TaskId);

            try
            {
                await _taskRepository.DeleteTaskAsync(request.TaskId);
                _logger.Information("[DeleteTaskHandler] Tarefa com Id: {TaskId} excluída do bando de dados", request.TaskId);

                await _elasticSearchRepository.RemoveTaskAsync(request.TaskId);
                _logger.Information("[DeleteTaskHandler] Tarefa com TaskId: {TaskId} removida do ElasticSearch", request.TaskId);

                await _cacheService.RemoveCacheAsync(request.TaskId.ToString());
                _logger.Information("[DeleteTaskHandler] Tarefa com TaskId: {TaskId} removida do cache Redis", request.TaskId);

                return new DeleteTaskResponse { Success = true, Message = "Tarefa excluída com sucesso!" };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[DeleteTaskHandler] Erro ao deletar Tarefa com TaskId: {TaskId}", request.TaskId);
                throw;
            }
        }
    }
}