using Microsoft.EntityFrameworkCore;
using RequiredProj.Core.Data;
using RequiredProj.Core.Entities;

namespace RequiredProj.Core.Services;

public class TaskService(AppDbContext db)
{
    public async Task<List<TaskItem>> GetAllAsync()
        => await db.TaskItems.OrderByDescending(t => t.CreatedAt).ToListAsync();

    public async Task<ServiceResult<TaskItem>> GetAsync(int id)
    {
        TaskItem? item = await db.TaskItems.FindAsync(id);
        ServiceResult<TaskItem> result = item is null ? 
            ServiceResult<TaskItem>.NotFound() : 
            ServiceResult<TaskItem>.Success(item);
        
        return result;
    }

    public async Task<ServiceResult<TaskItem>> CreateAsync(TaskItem item)
    {
        item.CreatedAt = DateTime.UtcNow;
        if (item.DueDate.HasValue)
        {
            // Postgres requires DateTime to be explicitly UTC
            item.DueDate = DateTime.SpecifyKind(item.DueDate.Value, DateTimeKind.Utc);
            if (item.DueDate.Value.Date < DateTime.UtcNow.Date)
                return ServiceResult<TaskItem>.Failure("Due date cannot be in the past");
        }
        
        db.TaskItems.Add(item);
        await db.SaveChangesAsync();
        
        return ServiceResult<TaskItem>.Success(item);
    }

    public async Task<ServiceResult<TaskItem>> UpdateAsync(int id, TaskItem input)
    {
        TaskItem? item = await db.TaskItems.FindAsync(id);
        if (item is null) return ServiceResult<TaskItem>.NotFound();

        if (input.DueDate.HasValue && input.DueDate.Value.Date < DateTime.UtcNow.Date)
            return ServiceResult<TaskItem>.Failure("Due date cannot be in the past");

        item.Title = input.Title;
        item.Description = input.Description;
        item.Status = input.Status;
        item.DueDate = input.DueDate.HasValue
            ? DateTime.SpecifyKind(input.DueDate.Value, DateTimeKind.Utc)
            : null;
        await db.SaveChangesAsync();
        
        return ServiceResult<TaskItem>.Success(item);
    }

    public async Task<ServiceResult<TaskItem>> UpdateStatusAsync(int id, TaskItemStatus status)
    {
        TaskItem? item = await db.TaskItems.FindAsync(id);
        if (item is null) return ServiceResult<TaskItem>.NotFound();

        item.Status = status;
        await db.SaveChangesAsync();
        
        return ServiceResult<TaskItem>.Success(item);
    }

    public async Task<ServiceResult<TaskItem>> DeleteAsync(int id)
    {
        TaskItem? item = await db.TaskItems.FindAsync(id);
        if (item is null) return ServiceResult<TaskItem>.NotFound();

        db.TaskItems.Remove(item);
        await db.SaveChangesAsync();
        
        return ServiceResult<TaskItem>.Success(item);
    }
}
