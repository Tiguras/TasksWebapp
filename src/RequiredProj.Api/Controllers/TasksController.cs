using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequiredProj.Core.Data;
using RequiredProj.Core.Entities;

namespace RequiredProj.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TaskItem>>> GetAll()
        => await db.TaskItems.OrderByDescending(t => t.CreatedAt).ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItem>> Get(int id)
    {
        TaskItem? item = await db.TaskItems.FindAsync(id);
        return item is null ? NotFound() : item;
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> Create(TaskItem item)
    {
        item.CreatedAt = DateTime.UtcNow;
        if (item.DueDate.HasValue)
        {
            item.DueDate = DateTime.SpecifyKind(item.DueDate.Value, DateTimeKind.Utc);
            if (item.DueDate.Value.Date < DateTime.UtcNow.Date)
                return BadRequest(new { error = "Due date cannot be in the past" });
        }
        db.TaskItems.Add(item);
        await db.SaveChangesAsync();
        return Ok(item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TaskItem input)
    {
        TaskItem? item = await db.TaskItems.FindAsync(id);
        if (item is null) return NotFound();

        item.Title = input.Title;
        item.Description = input.Description;
        item.Status = input.Status;
        item.DueDate = input.DueDate.HasValue
            ? DateTime.SpecifyKind(input.DueDate.Value, DateTimeKind.Utc)
            : null;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        TaskItem? item = await db.TaskItems.FindAsync(id);
        if (item is null) return NotFound();

        item.Status = request.Status;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.TaskItems.FindAsync(id);
        if (item is null) return NotFound();

        db.TaskItems.Remove(item);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

public record UpdateStatusRequest(TaskItemStatus Status);
