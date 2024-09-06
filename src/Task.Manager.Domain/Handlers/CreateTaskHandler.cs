using MediatR;
using Task.Manager.Shareable.Requests;
using Task.Manager.Shareable.Responses;

namespace Task.Manager.Domain.Handlers
{
    public class CreateTaskHandler : IRequestHandler<CreateTaskRequest, CreateTaskResponse>
    {
        //private readonly ITaskRepository _taskRepository;

        //public CreateTaskHandler(ITaskRepository taskRepository)
        //{
        //    _taskRepository = taskRepository;
        //}

        public async Task<CreateTaskResponse> Handle(CreateTaskRequest request, CancellationToken cancellationToken)
        {
            //var task = new TaskItem
            //{
            //    Name = request.Name,
            //    Description = request.Description,
            //    CreatedAt = DateTime.UtcNow
            //};

            //await _taskRepository.AddAsync(task);

            //return task.Id;

            return new CreateTaskResponse { TaskId = Guid.NewGuid() };
        }
    }
}