using Microsoft.EntityFrameworkCore;
using TaskProject.Core.Data;
using TaskProject.Core.Entities;
using TaskProject.Core.Services;

namespace TaskProject.Tests;

[TestFixture]
public class TaskServiceTests
{
    private TaskService _sut;
    private AppDbContext dbContext;

    [SetUp]
    public void Setup()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        dbContext = new AppDbContext(optionsBuilder.Options);
        dbContext.Database.EnsureCreated();
        _sut = new TaskService(dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Dispose();
    }

    [Test]
    // Unit test naming convention from Unit Testing: Principles, Practices and Patterns by V. Khorikov
    public async Task GetAsync_with_nothing_in_db_returns_IsNotFound_as_true()
    {
        var result = await _sut.GetAsync(1);

        Assert.That(result.Value, Is.Null);
        Assert.That(result.IsNotFound, Is.True);
    }

    [Test]
    public async Task GetAsync_with_existing_item_returns_it()
    {
        dbContext.TaskItems.Add(new TaskItem
        {
            Id = 1,
            Title = "Test1",
            Description = "Test description",
            DueDate = DateTime.Today.AddDays(1)
        });
        await dbContext.SaveChangesAsync();

        var result = await _sut.GetAsync(1);

        Assert.That(result.Value, Is.TypeOf<TaskItem>());
        Assert.That(result.Value!.Description, Is.EqualTo("Test description"));
        Assert.That(result.Value.Title, Is.EqualTo("Test1"));
        Assert.That(result.Value.DueDate, Is.EqualTo(DateTime.Today.AddDays(1)));
    }

    [Test]
    public async Task CreateAsync_with_valid_item_returns_success()
    {
        var result = await _sut.CreateAsync(new TaskItem { Title = "New task" });

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value!.Title, Is.EqualTo("New task"));
    }

    [Test]
    public async Task CreateAsync_with_past_due_date_returns_failure()
    {
        var result = await _sut.CreateAsync(new TaskItem { Title = "New task", DueDate = DateTime.UtcNow.AddDays(-1) });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Due date cannot be in the past"));
    }

    [Test]
    public async Task UpdateAsync_with_existing_item_returns_success()
    {
        dbContext.TaskItems.Add(new TaskItem { Id = 1, Title = "Original" });
        await dbContext.SaveChangesAsync();

        var result = await _sut.UpdateAsync(1, new TaskItem { Title = "Updated", Status = TaskItemStatus.InProgress });

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value!.Title, Is.EqualTo("Updated"));
        Assert.That(result.Value.Status, Is.EqualTo(TaskItemStatus.InProgress));
    }

    [Test]
    public async Task UpdateAsync_with_nothing_in_db_returns_IsNotFound_as_true()
    {
        var result = await _sut.UpdateAsync(1, new TaskItem { Title = "Updated" });

        Assert.That(result.IsNotFound, Is.True);
    }

    [Test]
    public async Task UpdateAsync_with_past_due_date_returns_failure()
    {
        dbContext.TaskItems.Add(new TaskItem { Id = 1, Title = "Original" });
        await dbContext.SaveChangesAsync();

        var result = await _sut.UpdateAsync(1, new TaskItem { Title = "Updated", DueDate = DateTime.UtcNow.AddDays(-1) });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Due date cannot be in the past"));
    }

    [Test]
    public async Task UpdateStatusAsync_with_existing_item_returns_success()
    {
        dbContext.TaskItems.Add(new TaskItem { Id = 1, Title = "Task" });
        await dbContext.SaveChangesAsync();

        var result = await _sut.UpdateStatusAsync(1, TaskItemStatus.Completed);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value!.Status, Is.EqualTo(TaskItemStatus.Completed));
    }

    [Test]
    public async Task UpdateStatusAsync_with_nothing_in_db_returns_IsNotFound_as_true()
    {
        var result = await _sut.UpdateStatusAsync(1, TaskItemStatus.Completed);

        Assert.That(result.IsNotFound, Is.True);
    }

    [Test]
    public async Task DeleteAsync_with_existing_item_returns_success()
    {
        dbContext.TaskItems.Add(new TaskItem { Id = 1, Title = "Task" });
        await dbContext.SaveChangesAsync();

        var result = await _sut.DeleteAsync(1);

        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task DeleteAsync_with_nothing_in_db_returns_IsNotFound_as_true()
    {
        var result = await _sut.DeleteAsync(1);

        Assert.That(result.IsNotFound, Is.True);
    }
}
