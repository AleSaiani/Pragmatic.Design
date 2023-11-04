using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pragmatic.Design.DataProcessor.Persistence;

class TaskExecutionConfiguration : IEntityTypeConfiguration<TaskExecution>
{
    public void Configure(EntityTypeBuilder<TaskExecution> builder)
    {
        builder.ToTable(nameof(TaskExecution));
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasMaxLength(256);
        builder.Property(m => m.Name).HasMaxLength(256);
        builder.Property(m => m.TaskType).HasMaxLength(256);
        builder.Property(m => m.State).HasMaxLength(256);

        builder.HasIndex(nameof(TaskExecution.State), nameof(TaskExecution.TaskType));
        builder.HasIndex(nameof(TaskExecution.DataProcessorJobStartTime), nameof(TaskExecution.Name), nameof(TaskExecution.TaskType));
    }
}
