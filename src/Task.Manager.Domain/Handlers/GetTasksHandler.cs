using MediatR;
using Task.Manager.Domain.Repositories;
using Task.Manager.Shareable.Requests;
using Task.Manager.Shareable.Responses;

namespace Task.Manager.Domain.Handlers
{
    internal class GetTasksHandler : IRequestHandler<GetTaskRequest, GetTaskResponse>
    {
        private readonly ITaskRepository _taskRepository;

        public GetTasksHandler( ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<GetTaskResponse> Handle(GetTaskRequest request, CancellationToken cancellationToken)
        {
            var taskEntity = _taskRepository.GetTaskByIdAsync(request.TaskId);

            if (taskEntity.Result == null) 
            {
                return new GetTaskResponse();
            }

            return new GetTaskResponse 
            {
                Id = taskEntity.Result.Id,
                Title = taskEntity.Result.Title,
                Description = taskEntity.Result.Description,
                IsCompleted = taskEntity.Result.IsCompleted,
                CreatedAt = taskEntity.Result.CreatedAt,
                UpdatedAt = taskEntity.Result.UpdatedAt,
            };
        }
    }
}