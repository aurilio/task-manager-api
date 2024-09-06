using MediatR;
using Task.Manager.Domain.Repositories;
using Task.Manager.Shareable.Requests;
using Task.Manager.Shareable.Responses;

namespace Task.Manager.Domain.Handlers
{
    public class UpdateTaskHandler : IRequestHandler<UpdateTaskRequest, UpdateTaskResponse>
    {
        private readonly ITaskRepository _taskRepository;

        public UpdateTaskHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<UpdateTaskResponse> Handle(UpdateTaskRequest request, CancellationToken cancellationToken)
        {
            var taskEntity = await _taskRepository.GetTaskByIdAsync(request.TaskEntityDTO.Id);

            if (taskEntity == null)
            {
                return new UpdateTaskResponse { Success = false, Message = "Tarefa não encontrada." };
            }

            taskEntity.Title = request.TaskEntityDTO.Title;
            taskEntity.Description = request.TaskEntityDTO.Description;
            taskEntity.IsCompleted = request.TaskEntityDTO.IsCompleted;
            taskEntity.UpdatedAt = DateTime.UtcNow;

            _taskRepository.UpdateTaskAsync(taskEntity);

            return new UpdateTaskResponse { Success = true, Message = "Tarefa atualizada com sucesso!" };
        }
    }
}