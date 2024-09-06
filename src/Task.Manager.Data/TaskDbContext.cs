using Microsoft.EntityFrameworkCore;
using Task.Manager.Domain.Entities;

namespace Task.Manager.Data;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options)
        : base(options) { }

    public DbSet<TaskEntity> TaskEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskEntity>().HasKey(t => t.Id);
        base.OnModelCreating(modelBuilder);
    }
}