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
            _logger.Information("[GetTasksBySearchHandler] Iniciando busca de tarefas no Elasticsearch com o termo: {SearchTerm}, Página: {PageNumber}, Tamanho da página: {PageSize}",
                request.Search, request.PageNumber, request.PageSize);

            try
            {
                var totalTasks = await _elasticSearchRepository.GetTotalTaskCountAsync(request.Search);
                var totalPages = (int)Math.Ceiling(totalTasks / (double)request.PageSize);

                var tasks = await _elasticSearchRepository.SearchTasksAsync(request.Search, request.PageNumber, request.PageSize);

                if (!tasks.Any())
                {
                    _logger.Warning("[GetTasksBySearchHandler] Nenhuma tarefa encontrada para o termo: {SearchTerm}.", request.Search);
                    return new GetTasksBySearchResponse { Tasks = new List<TaskEntityDTO>(), TotalPages = totalPages, CurrentPage = request.PageNumber };
                }

                var taskDtos = tasks.Select(task => new TaskEntityDTO
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    IsCompleted = task.IsCompleted,
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt
                }).ToList();

                _logger.Information("[GetTasksBySearchHandler] Busca de tarefas concluída com sucesso. {TaskCount} tarefas encontradas.", taskDtos.Count);

                return new GetTasksBySearchResponse
                {
                    Tasks = taskDtos,
                    TotalPages = totalPages,
                    CurrentPage = request.PageNumber
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[GetTasksBySearchHandler] Erro ao buscar tarefas no Elasticsearch com o termo: {Search}", request.Search);
                throw;
            }
        }
    }
}