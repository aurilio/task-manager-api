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
            _logger.Information("[GetAllTasksHandler] Iniciando a busca de todas as tarefas.");

            var tasks = await _taskRepository.GetAllTasksAsync();
            if (tasks == null || !tasks.Any())
            {
                _logger.Warning("[GetAllTasksHandler] Nenhuma tarefa encontrada no banco de dados.");
                return new GetAllTaskResponse { Tasks = new List<TaskEntityDTO>() };
            }

            _logger.Information("[GetAllTasksHandler] {TaskCount} tarefas encontradas no banco de dados.", tasks.Count());

            var taskDto = new TaskEntityDTO();
            var listTaskDto = new List<TaskEntityDTO>();

            foreach (var item in tasks)
            {
                taskDto.Id = item.Id;
                taskDto.Title = item.Title;
                taskDto.Description = item.Description;
                taskDto.IsCompleted = item.IsCompleted;
                taskDto.CreatedAt = item.CreatedAt;
                taskDto.UpdatedAt = item.UpdatedAt;

                listTaskDto.Add(taskDto);
            }

            _logger.Information("[GetAllTasksHandler] Busca de todas as tarefas concluída com sucesso.");

            return new GetAllTaskResponse
            {
                Tasks = listTaskDto
            };
        }
    }
}