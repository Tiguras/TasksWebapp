using System.ComponentModel.DataAnnotations;

namespace RequiredProj.Core.Entities;

public enum TaskItemStatus
{
    Pending,
    InProgress,
    Completed
}

public class TaskItem
{
    public int Id { get; set; }
    
    public required string Title { get; set; }
    
    [MaxLength(500)] public string? Description { get; set; }
    
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
    
    public DateTime? DueDate { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}