using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Aornis.Tests;

public class ThrowIfNone : TestBase
{
    [Fact]
    public async Task ThrowsWhenNone_AsyncFunc()
    {
        await Assert.ThrowsAsync<Exception>(() => Optional<string>.Empty.ThrowIfEmptyAsync(() => Task.FromResult(new Exception("What"))));
    }

    [Fact]
    public async Task DoesNotThrowWhenSome_AsyncFunc()
    {
        bool explode = true;
        var value = (await Optional.Of(1).ThrowIfEmptyAsync(() =>
        {
            if (explode)
            {
                throw new Exception("This function should not be called!");
            }

            return Task.FromResult(new Exception("What"));
        })).Value;
        value.Should().Be(1);
    }
        
    [Fact]
    public void ThrowsWhenNone_SyncFunc()
    {
        Assert.Throws<Exception>(() => Optional<string>.Empty.ThrowIfEmpty(() => new Exception("What")));
    }

    [Fact]
    public void DoesNotThrowWhenSome_SyncFunc()
    {
        bool explode = true;
        var value = Optional.Of(1).ThrowIfEmpty(() =>
        {
            if (explode)
            {
                throw new Exception("This function should not be called!");
            }
            return new Exception("What");
        }).Value;
        value.Should().Be(1);
    }
        
    [Fact]
    public void ThrowsWhenNone_Value()
    {
        Assert.Throws<Exception>(() => Optional<int>.Empty.ThrowIfEmpty(new Exception("What")));
    }

    [Fact]
    public void DoesNotThrowWhenSome_Value()
    {
        var value = Optional.Of(1).ThrowIfEmpty(new Exception("What")).Value;
        value.Should().Be(1);
    }
}