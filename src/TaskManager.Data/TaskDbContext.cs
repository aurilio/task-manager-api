using Microsoft.EntityFrameworkCore;
using TaskManager.Data.EntityConfigurations;
using TaskManager.Domain.Entities;

namespace TaskManager.Data;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options)
        : base(options) { }

    public DbSet<TaskEntity> TaskEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TaskEntityConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}