using Task.Manager.Domain.Entities;

namespace Task.Manager.Domain.Repositories;

public interface ITaskRepository
{
    Task<IEnumerable<TaskEntity>> GetAllTasksAsync();

    Task<TaskEntity?> GetTaskByIdAsync(Guid id);

    void AddTaskAsync(TaskEntity task);

    void UpdateTaskAsync(TaskEntity task);

    void DeleteTaskAsync(Guid id);
}