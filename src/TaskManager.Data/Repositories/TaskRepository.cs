using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Data.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _context;

    public TaskRepository(TaskDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskEntity>> GetAllTasksAsync()
    {
        return await _context.TaskEntities.ToListAsync();
    }

    public async Task<TaskEntity?> GetTaskByIdAsync(Guid id)
    {
        return await _context.TaskEntities.FindAsync(id);
    }

    public async Task AddTaskAsync(TaskEntity task)
    {
        _context.TaskEntities.Add(task);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTaskAsync(TaskEntity task)
    {
        _context.TaskEntities.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(Guid id)
    {
        var task = await _context.TaskEntities.FindAsync(id);
        if (task != null)
        {
            _context.TaskEntities.Remove(task);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<TaskEntity>> GetTasksWithPaginationAsync(int pageNumber, int pageSize)
    {
        return await _context.TaskEntities
            .OrderBy(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalTasksCountAsync()
    {
        return await _context.TaskEntities.CountAsync();
    }
}