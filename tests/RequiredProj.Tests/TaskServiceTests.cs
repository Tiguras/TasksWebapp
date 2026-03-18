using Microsoft.EntityFrameworkCore;
using RequiredProj.Core.Data;
using RequiredProj.Core.Entities;
using RequiredProj.Core.Services;

namespace RequiredProj.Tests;

[TestFixture]
public class TaskServiceTests
{
    private TaskService _sut;
    private AppDbContext dbContext;
    
    [SetUp]
    public void Setup()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase("TestDb");
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
    public void GetAsync_with_nothing_in_db_returns_IsNotFound_as_true()
    {
        var result = _sut.GetAsync(1);

        Assert.That(result.Result.Value, Is.Null);
        Assert.That(result.Result.IsNotFound, Is.True);
    }

    [Test]
    public void GetAsync_with_existing_item_returns_it()
    {
        dbContext.TaskItems.Add(new TaskItem
        {
            Id = 1,
            Title = "Test1",
            Description = "Test description",
            DueDate = DateTime.Today.AddDays(1)
        });
        dbContext.SaveChanges();

        var result = _sut.GetAsync(1);

        Assert.That(result.Result.Value, Is.TypeOf<TaskItem>());
        Assert.That(result.Result.Value.Description, Is.EqualTo("Test description"));
        Assert.That(result.Result.Value.Title, Is.EqualTo("Test1"));
        Assert.That(result.Result.Value.DueDate, Is.EqualTo(DateTime.Today.AddDays(1)));
    }

    [Test]
    public void CreateAsync_with_valid_item_returns_success()
    {
        var result = _sut.CreateAsync(new TaskItem { Title = "New task" });

        Assert.That(result.Result.IsSuccess, Is.True);
        Assert.That(result.Result.Value.Title, Is.EqualTo("New task"));
    }

    [Test]
    public void CreateAsync_with_past_due_date_returns_failure()
    {
        var result = _sut.CreateAsync(new TaskItem { Title = "New task", DueDate = DateTime.UtcNow.AddDays(-1) });

        Assert.That(result.Result.IsSuccess, Is.False);
        Assert.That(result.Result.Error, Is.EqualTo("Due date cannot be in the past"));
    }

    [Test]
    public void UpdateAsync_with_existing_item_returns_success()
    {
        dbContext.TaskItems.Add(new TaskItem { Id = 1, Title = "Original" });
        dbContext.SaveChanges();

        var result = _sut.UpdateAsync(1, new TaskItem { Title = "Updated", Status = TaskItemStatus.InProgress });

        Assert.That(result.Result.IsSuccess, Is.True);
        Assert.That(result.Result.Value.Title, Is.EqualTo("Updated"));
        Assert.That(result.Result.Value.Status, Is.EqualTo(TaskItemStatus.InProgress));
    }

    [Test]
    public void UpdateAsync_with_nothing_in_db_returns_IsNotFound_as_true()
    {
        var result = _sut.UpdateAsync(1, new TaskItem { Title = "Updated" });

        Assert.That(result.Result.IsNotFound, Is.True);
    }

    [Test]
    public void UpdateAsync_with_past_due_date_returns_failure()
    {
        dbContext.TaskItems.Add(new TaskItem { Id = 1, Title = "Original" });
        dbContext.SaveChanges();

        var result = _sut.UpdateAsync(1, new TaskItem { Title = "Updated", DueDate = DateTime.UtcNow.AddDays(-1) });

        Assert.That(result.Result.IsSuccess, Is.False);
        Assert.That(result.Result.Error, Is.EqualTo("Due date cannot be in the past"));
    }

    [Test]
    public void UpdateStatusAsync_with_existing_item_returns_success()
    {
        dbContext.TaskItems.Add(new TaskItem { Id = 1, Title = "Task" });
        dbContext.SaveChanges();

        var result = _sut.UpdateStatusAsync(1, TaskItemStatus.Completed);

        Assert.That(result.Result.IsSuccess, Is.True);
        Assert.That(result.Result.Value.Status, Is.EqualTo(TaskItemStatus.Completed));
    }

    [Test]
    public void UpdateStatusAsync_with_nothing_in_db_returns_IsNotFound_as_true()
    {
        var result = _sut.UpdateStatusAsync(1, TaskItemStatus.Completed);

        Assert.That(result.Result.IsNotFound, Is.True);
    }

    [Test]
    public void DeleteAsync_with_existing_item_returns_success()
    {
        dbContext.TaskItems.Add(new TaskItem { Id = 1, Title = "Task" });
        dbContext.SaveChanges();

        var result = _sut.DeleteAsync(1);

        Assert.That(result.Result.IsSuccess, Is.True);
    }

    [Test]
    public void DeleteAsync_with_nothing_in_db_returns_IsNotFound_as_true()
    {
        var result = _sut.DeleteAsync(1);

        Assert.That(result.Result.IsNotFound, Is.True);
    }
}