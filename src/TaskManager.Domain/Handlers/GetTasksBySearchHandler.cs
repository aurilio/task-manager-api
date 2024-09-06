using MediatR;
using Serilog;
using TaskManager.Domain.Repositories;
using TaskManager.Shareable.DTOs;
using TaskManager.Shareable.Requests;
using TaskManager.Shareable.Responses;

namespace TaskManager.Domain.Handlers
{
    public class GetTasksBySearchHandler : IRequestHandler<GetTasksBySearchRequest, GetTasksBySearchResponse>
    {
        private readonly IElasticSearchRepository _elasticSearchRepository;
        private readonly ILogger _logger;

        public GetTasksBySearchHandler(
            IElasticSearchRepository elasticSearchRepository,
            ILogger logger)
        {
            _elasticSearchRepository = elasticSearchRepository;
            _logger = logger;
        }

        public async Task<GetTasksBySearchResponse> Handle(GetTasksBySearchRequest request, CancellationToken cancellationToken)
        {
            _logger.Information("[GetTasksBySearchHandler] Iniciando busca de tarefas no Elasticsearch com o termo: {Search}", request.Search);

            try
            {
                var tasks = await _elasticSearchRepository.SearchTasksAsync(request.Search);

                var taskDtos = tasks.Select(task => new TaskEntityDTO
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    IsCompleted = task.IsCompleted,
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt
                }).ToList();

                _logger.Information("[GetTasksBySearchHandler] Busca de tarefas concluída com sucesso. {Count} tarefas encontradas.", taskDtos.Count);

                return new GetTasksBySearchResponse { Tasks = taskDtos };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[GetTasksBySearchHandler] Erro ao buscar tarefas no Elasticsearch com o termo: {SearchTerm}", request.Search);
                throw;
            }
        }
    }
}