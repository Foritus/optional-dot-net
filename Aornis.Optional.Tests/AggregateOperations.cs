using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Aornis.Tests;

public class AggregateOperations
{
    private IEnumerable<Optional<int>> AllPresent =>
        Enumerable.Range(0, 10)
                  .Select(Optional.Of);

    private IEnumerable<Optional<int>> PartialPresent =>
        Enumerable.Range(0, 10)
            .Select(x =>
            {
                if (x % 2 == 0)
                {
                    return Optional.Empty;
                }
                return Optional.Of(x);
            });

    private IEnumerable<Optional<int>> NonePresent =>
        Enumerable.Range(0, 10)
            .Select(_ => Optional<int>.Empty);

    [Fact]
    public void AllValuesPresent()
    {
        Optional.All(AllPresent.ToArray()).Should().BeTrue();
        Optional.All(AllPresent.Cast<IOptional>()).Should().BeTrue();
        Optional.All((IOptional)Optional.Of(1), Optional.Of(2)).Should().BeTrue();
    }

    [Fact]
    public void AllValuesPartial()
    {
        Optional.All(PartialPresent.ToArray()).Should().BeFalse();
        Optional.All(PartialPresent.Cast<IOptional>()).Should().BeFalse();
        Optional.All((IOptional)Optional.Of(1), Optional<int>.Empty).Should().BeFalse();
    }

    [Fact]
    public void AllValuesNone()
    {
        Optional.All(NonePresent.ToArray()).Should().BeFalse();
        Optional.All(NonePresent.Cast<IOptional>()).Should().BeFalse();
        Optional.All(Optional<int>.Empty, Optional<int>.Empty).Should().BeFalse();
    }

    [Fact]
    public void AnyValuesPresent()
    {
        Optional.Any(AllPresent.ToArray()).Should().BeTrue();
        Optional.Any(AllPresent.Cast<IOptional>()).Should().BeTrue();
        Optional.Any((IOptional)Optional.Of(1), Optional.Of(2)).Should().BeTrue();
    }

    [Fact]
    public void AnyValuesPartial()
    {
        Optional.Any(PartialPresent.ToArray()).Should().BeTrue();
        Optional.Any(PartialPresent.Cast<IOptional>()).Should().BeTrue();
        Optional.Any((IOptional)Optional.Of(1), Optional<int>.Empty).Should().BeTrue();
    }

    [Fact]
    public void AnyValuesNone()
    {
        Optional.Any(NonePresent.ToArray()).Should().BeFalse();
        Optional.Any(NonePresent.Cast<IOptional>()).Should().BeFalse();
        Optional.Any(Optional<int>.Empty, Optional<int>.Empty).Should().BeFalse();
    }

    [Fact]
    public void GetFirstCallback()
    {
        var callbacks = new[]
        {
            () => Optional.Of(12345),
            () => Optional<int>.Empty,
        };

        Optional.Get(callbacks).Value.Should().Be(12345);
    }

    [Fact]
    public void GetLastCallback()
    {
        var callbacks = new[]
        {
            () => Optional<int>.Empty,
            () => Optional<int>.Empty,
            () => Optional.Of(456789),
        };

        Optional.Get(callbacks).Value.Should().Be(456789);
    }

    [Fact]
    public void GetNoCallback()
    {
        var callbacks = new[]
        {
            () => Optional<int>.Empty,
            () => Optional<int>.Empty,
        };

        Optional.Get(callbacks).HasValue.Should().BeFalse();
    }

    [Fact]
    public void GetEmptyCallbackList()
    {
        Optional.Get(Array.Empty<Func<Optional<int>>>()).HasValue.Should().BeFalse();
    }
}