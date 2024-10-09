using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Aornis.Tests;

public class IfPresent
{
    private const string initialValue = "hello";
    private readonly Optional<string> value;

    public IfPresent()
    {
        value = Optional.Of(initialValue);
    }

    #region Sync

    [Fact]
    public void IfPresentDoesNotCallFuncWhenEmpty()
    {
        Optional<string>.Empty.IfPresent(new Action<string>(x => throw new Exception("This function should not be called")));
    }

    [Fact]
    public void IfPresentCallsFuncWhenValueIsPresent()
    {
        bool wasCalled = false;
        value.IfPresent(x => wasCalled = true);

        wasCalled.Should().BeTrue();
    }

    [Fact]
    public void IfPresentReturnsOptional()
    {
        value.IfPresent(x => Console.WriteLine(x)).Should().Be(value);
    }

    #endregion

    #region Async

    [Fact]
    public async Task IfPresentAsyncDoesNotCallFuncWhenEmpty()
    {
        await Optional<string>.Empty.IfPresentAsync(x => throw new Exception("This function should not be called"));
    }

    [Fact]
    public async Task IfPresentAsyncCallsFuncWhenValueIsPresent()
    {
        bool wasCalled = false;
        await value.IfPresentAsync(x =>
        {
            wasCalled = true;
            return Task.CompletedTask;
        });

        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task IfPresentAsyncReturnsOptional()
    {
        (await value.IfPresentAsync(x => Task.CompletedTask)).Should().Be(value);
    }

    #endregion
}