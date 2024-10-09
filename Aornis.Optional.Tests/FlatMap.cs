using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Aornis.Tests;

public class FlatMap
{
    const string initialValue = "Test";
    const string newValue = "bees";

    private readonly Optional<string> value;

    public FlatMap()
    {
        value = Optional.Of(initialValue);
    }

    #region Sync

    [Fact]
    public void FlatMapReturnsValueCreatedByFunc()
    {
        value.FlatMap(x => Optional.Of(newValue)).Should().Be(Optional.Of(newValue));
    }

    [Fact]
    public void FlatMapReturnsEmptyWhenFuncReturnsEmpty()
    {
        value.FlatMap(x => Optional<string>.Empty).Should().Be(Optional<string>.Empty);
    }

    [Fact]
    public void FlatMapDoesNotCallFuncWhenValueIsEmpty()
    {
        Optional<string>.Empty.FlatMap(new Func<string, Optional<string>>(x => throw new Exception("This function should not be called")));
    }

    #endregion

    #region Async

    [Fact]
    public async Task FlatMapAsyncReturnsValueCreatedByFunc()
    {
        (await value.FlatMapAsync(x => Task.FromResult(Optional.Of(x + newValue)))).Should().Be(Optional.Of(initialValue + newValue));
    }

    [Fact]
    public async Task FlatMapAsyncReturnsEmptyWhenFuncReturnsEmpty()
    {
        (await value.FlatMapAsync(x => Task.FromResult(Optional<string>.Empty))).Should().Be(Optional<string>.Empty);
    }

    [Fact]
    public async Task FlatMapAsyncDoesNotCallFuncWhenValueIsEmpty()
    {
        await Optional<string>.Empty.FlatMapAsync(new Func<string, Task<Optional<string>>>(x => throw new Exception("This function should not be called")));
    }

    #endregion
}