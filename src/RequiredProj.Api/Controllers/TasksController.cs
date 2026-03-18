using Microsoft.AspNetCore.Mvc;
using RequiredProj.Core.Entities;
using RequiredProj.Core.Services;

namespace RequiredProj.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController(TaskService taskService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TaskItem>>> GetAll()
        => await taskService.GetAllAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItem>> Get(int id)
    {
        var result = await taskService.GetAsync(id);
        return result.IsNotFound ? NotFound() : result.Value!;
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> Create(TaskItem item)
    {
        var result = await taskService.CreateAsync(item);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TaskItem input)
    {
        var result = await taskService.UpdateAsync(id, input);
        if (result.IsNotFound) return NotFound();
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return NoContent();
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        var result = await taskService.UpdateStatusAsync(id, request.Status);
        return result.IsNotFound ? NotFound() : NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await taskService.DeleteAsync(id);
        return result.IsNotFound ? NotFound() : NoContent();
    }
}

public record UpdateStatusRequest(TaskItemStatus Status);
