using MediatR;
using Task.Manager.Domain.Entities;
using Task.Manager.Domain.Repositories;
using Task.Manager.Shareable.Requests;
using Task.Manager.Shareable.Responses;

namespace Task.Manager.Domain.Handlers
{
    public class CreateTaskHandler : IRequestHandler<CreateTaskRequest, CreateTaskResponse>
    {
        private readonly ITaskRepository _taskRepository;

        public CreateTaskHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<CreateTaskResponse> Handle(CreateTaskRequest request, CancellationToken cancellationToken)
        {
            var taskEntity = new TaskEntity
            {
                Id = Guid.NewGuid(),
                Title = request.TaskEntityDTO.Title,
                Description = request.TaskEntityDTO.Description,
                IsCompleted = request.TaskEntityDTO.IsCompleted,
                CreatedAt = DateTime.UtcNow
            };

            _taskRepository.AddTaskAsync(taskEntity);

            return new CreateTaskResponse { TaskId = taskEntity.Id };
        }
    }
}