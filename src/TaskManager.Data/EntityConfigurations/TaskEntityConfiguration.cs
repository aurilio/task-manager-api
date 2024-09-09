using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Data.EntityConfigurations;

public class TaskEntityConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.ToTable("TaskEntity");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(t => t.Description)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(t => t.IsCompleted);

        builder.Property(t => t.CreatedAt)
               .IsRequired();

        builder.Property(t => t.UpdatedAt);
    }
}