using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Repositories;

public interface ITaskRepository
{
    Task<IEnumerable<TaskEntity>> GetAllTasksAsync();

    Task<TaskEntity?> GetTaskByIdAsync(Guid id);

    Task AddTaskAsync(TaskEntity task);

    Task UpdateTaskAsync(TaskEntity task);

    Task DeleteTaskAsync(Guid id);
}