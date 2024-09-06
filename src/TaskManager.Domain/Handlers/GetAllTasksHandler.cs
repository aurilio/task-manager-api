using MediatR;
using Serilog;
using TaskManager.Domain.Repositories;
using TaskManager.Shareable.DTOs;
using TaskManager.Shareable.Requests;
using TaskManager.Shareable.Responses;

namespace TaskManager.Domain.Handlers
{
    public class GetAllTasksHandler : IRequestHandler<GetAllTasksRequest, GetAllTaskResponse>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger _logger;

        public GetAllTasksHandler(
            ITaskRepository taskRepository,
            ILogger logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        public async Task<GetAllTaskResponse> Handle(GetAllTasksRequest request, CancellationToken cancellationToken)
        {
            _logger.Information("[GetAllTasksHandler] Iniciando a busca de todas as tarefas com paginação. Página: {PageNumber}, Tamanho: {PageSize}", request.PageNumber, request.PageSize);

            var totalTasksCount = await _taskRepository.GetTotalTasksCountAsync();
            var totalPages = (int)Math.Ceiling(totalTasksCount / (double)request.PageSize);

            var tasks = await _taskRepository.GetTasksWithPaginationAsync(request.PageNumber, request.PageSize);

            if (tasks == null || !tasks.Any())
            {
                _logger.Warning("[GetAllTasksHandler] Nenhuma tarefa encontrada no banco de dados.");
                return new GetAllTaskResponse { Tasks = new List<TaskEntityDTO>(), TotalPages = totalPages, CurrentPage = request.PageNumber };
            }

            _logger.Information("[GetAllTasksHandler] {TaskCount} tarefas encontradas no banco de dados.", tasks.Count());

            var taskDtos = tasks.Select(task => new TaskEntityDTO
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            }).ToList();

            _logger.Information("[GetAllTasksHandler] Busca de todas as tarefas concluída com sucesso.");

            return new GetAllTaskResponse
            {
                Tasks = taskDtos,
                TotalPages = totalPages,
                CurrentPage = request.PageNumber
            };
        }
    }
}