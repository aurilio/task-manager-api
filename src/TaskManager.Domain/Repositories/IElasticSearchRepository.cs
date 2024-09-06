using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Repositories
{
    public interface IElasticSearchRepository
    {
        Task SetTaskAsync(TaskEntity task);

        Task<TaskEntity> GetTaskByIdAsync(string taskId);

        Task<IEnumerable<TaskEntity>> SearchTasksAsync(string search);

        Task RemoveTaskAsync(Guid taskId);
    }
}