using Microsoft.EntityFrameworkCore;
using Task.Manager.Domain.Entities;
using Task.Manager.Domain.Repositories;

namespace Task.Manager.Data.Repositories;

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

    public async void AddTaskAsync(TaskEntity task)
    {
        _context.TaskEntities.Add(task);
        await _context.SaveChangesAsync();
    }

    public async void UpdateTaskAsync(TaskEntity task)
    {
        _context.TaskEntities.Update(task);
        await _context.SaveChangesAsync();
    }

    public async void DeleteTaskAsync(Guid id)
    {
        var task = await _context.TaskEntities.FindAsync(id);
        if (task != null)
        {
            _context.TaskEntities.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}