using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Aornis.Tests;

public class IfPresentTaskExtensionsTest
{
    private Optional<int> baseValue;
    private Optional<int> newValue;

    public IfPresentTaskExtensionsTest()
    {
        baseValue = Optional.Of(123);
        newValue = Optional.Of(456);
    }

    [Fact]
    public async Task ChainTasks()
    {
        int result = 0;
        await Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            return Optional.Of(123);
        }).IfPresentAsync(async x =>
        {
            await Task.Delay(500);
            result = x * 2;
        });

        result.Should().Be(246);
    }

    [Fact]
    public async Task IfPresentOrElseAsync()
    {
        int result = await Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                return Optional<int>.Empty;
            }).IfPresentAsync(async x => throw new Exception("Should not be called"))
            .OrElseAsync(() => 456);

        result.Should().Be(456);
    }

    #region Sync

    [Fact]
    public async Task IfPresentOrElseSync()
    {
        int result = await Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                return Optional<int>.Empty;
            }).IfPresentAsync(_ => throw new Exception("Should not be called"))
            .OrElseAsync(() => 987);

        result.Should().Be(987);
    }

    [Fact]
    public async Task ReceivesValueCreatedByFunc()
    {
        int result = 0;
        await Task.FromResult(baseValue).IfPresentAsync(async x => result = x);
        result.Should().Be(baseValue.Value);
    }

    [Fact]
    public async Task DoesNotCallFuncWhenValueIsEmpty()
    {
        await Task.FromResult(Optional<string>.Empty).IfPresentAsync(x => throw new Exception("This function should not be called"));
    }

    #endregion

    #region Async

    [Fact]
    public async Task IfPresentAsyncReturnsValueCreatedByFunc()
    {
        int result = 0;
        await Task.FromResult(baseValue).IfPresentAsync(async x =>
        {
            newValue.IfPresent(async y => result = x + y);
        });

        result.Should().Be(baseValue.Value + newValue.Value);
    }

    [Fact]
    public async Task IfPresentAsyncDoesNotCallFuncWhenValueIsEmpty()
    {
        await Optional<string>.Empty.IfPresentAsync(x => throw new Exception("This function should not be called"));
    }

    [Fact]
    public async Task IfPresentAsyncObserveSideEffect()
    {
        var x = 123;
        await Task.FromResult(baseValue)
                .IfPresentAsync(async _ => x = 456);
        x.Should().Be(456);
    }

    [Fact]
    public async Task IfNotPresentSynchronousCallback()
    {
        int result = 0;
        await Task.FromResult(Optional<int>.Empty).IfNotPresentAsync(() =>
        {
            result = 159;
        });

        result.Should().Be(159);
    }

    [Fact]
    public async Task IfNotPresentAsyncReturnsValueCreatedByFunc()
    {
        int result = 0;
        await Task.FromResult(Optional<int>.Empty).IfNotPresentAsync(async () =>
        {
            result = 159;
        });

        result.Should().Be(159);
    }

    [Fact]
    public async Task IfNotPresentAsyncDoesNotCallFuncWhenValueIsPresent()
    {
        await Task.FromResult((Optional<string>)"hello").IfNotPresentAsync(() => throw new Exception("This function should not be called"));
    }

    [Fact]
    public async Task IfNotPresentAsyncObserveSideEffect()
    {
        var x = 123;
        await Task.FromResult(Optional<string>.Empty)
                .IfNotPresentAsync(async () =>
                {
                    x = 456;
                });
        x.Should().Be(456);
    }

    #endregion
}