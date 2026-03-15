using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequiredProj.Api.Controllers;
using RequiredProj.Core.Data;
using RequiredProj.Core.Entities;

namespace RequiredProj.Tests;

[TestFixture]
public class TasksControllerTests
{
    private AppDbContext _db = null!;
    private TasksController _controller = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _controller = new TasksController(_db);
    }

    [TearDown]
    public void TearDown() => _db.Dispose();

    [Test]
    public async Task GetAll_ReturnsEmptyList_WhenNoItems()
    {
        var result = await _controller.GetAll();
        Assert.That(result.Value, Is.Empty);
    }

    [Test]
    public async Task Create_ReturnsCreatedItem()
    {
        var item = new TaskItem { Title = "Test task", Description = "A description" };

        var result = await _controller.Create(item);

        Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
        var created = (CreatedAtActionResult)result.Result!;
        var task = (TaskItem)created.Value!;
        Assert.That(task.Title, Is.EqualTo("Test task"));
        Assert.That(task.Description, Is.EqualTo("A description"));
        Assert.That(task.Status, Is.EqualTo(TaskItemStatus.Pending));
        Assert.That(task.Id, Is.GreaterThan(0));
    }

    [Test]
    public async Task Get_ReturnsNotFound_WhenMissing()
    {
        var result = await _controller.Get(999);
        Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task UpdateStatus_ChangesOnlyStatus()
    {
        var item = new TaskItem { Title = "Status test" };
        _db.TaskItems.Add(item);
        await _db.SaveChangesAsync();

        var result = await _controller.UpdateStatus(item.Id, new UpdateStatusRequest(TaskItemStatus.InProgress));

        Assert.That(result, Is.TypeOf<NoContentResult>());
        var updated = await _db.TaskItems.FindAsync(item.Id);
        Assert.That(updated!.Status, Is.EqualTo(TaskItemStatus.InProgress));
    }

    [Test]
    public async Task Delete_RemovesItem()
    {
        var item = new TaskItem { Title = "To delete" };
        _db.TaskItems.Add(item);
        await _db.SaveChangesAsync();

        var result = await _controller.Delete(item.Id);

        Assert.That(result, Is.TypeOf<NoContentResult>());
        Assert.That(await _db.TaskItems.CountAsync(), Is.Zero);
    }
}
