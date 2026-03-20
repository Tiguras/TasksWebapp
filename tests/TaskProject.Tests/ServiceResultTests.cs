using TaskProject.Core.Services;

namespace TaskProject.Tests;

[TestFixture]
public class ServiceResultTests
{
    [Test]
    public void Success_should_return_with_error_null_and_isNotFound_false()
    {
        const string testString = "test";
        var test = ServiceResult<string>.Success(testString);

        Assert.That(test.IsNotFound, Is.False);
        Assert.That(test.IsSuccess, Is.True);
        Assert.That(test.Error, Is.EqualTo(null));
        Assert.That(test.Value, Is.EqualTo(testString));
    }

    [Test]
    public void NotFound_should_return_with_error_null_and_isNotFound_true()
    {
        var test = ServiceResult<string>.NotFound();

        Assert.That(test.IsNotFound, Is.True);
        Assert.That(test.IsSuccess, Is.False);
        Assert.That(test.Error, Is.Null);
        Assert.That(test.Value, Is.Null);
    }

    [Test]
    public void Failure_should_return_error_string_and_IsSuccess_false()
    {
        const string errorString = "there has been an error";
        var test = ServiceResult<string>.Failure(errorString);
        Assert.That(test.IsSuccess, Is.False);
        Assert.That(test.IsNotFound, Is.False);
        Assert.That(test.Error, Is.EqualTo(errorString));
    }
}