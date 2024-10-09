using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Aornis.Tests;

public class HashTests
{
    [Fact]
    public void UniqueValuesAreUnique()
    {
        var set = new HashSet<Optional<int>>();

        const int count = 100;
        for (int i = 0; i < count; ++i)
        {
            set.Add(Optional.Of(i));
        }

        set.Count.Should().Be(count);
    }

    [Fact]
    public void EqualValuesAreDeduplicated()
    {
        var set = new HashSet<Optional<int>>();
        
        for (int i = 0; i < 100; ++i)
        {
            set.Add(Optional.Of(1));
        }

        set.Count.Should().Be(1);
    }

    [Fact]
    public void EmptiesAreDeduplicated()
    {
        var set = new HashSet<Optional<string>>();

        for (int i = 0; i < 200; ++i)
        {
            set.Add(Optional.Empty);
        }

        set.Count.Should().Be(1);
    }

    [Fact]
    public void ValuesDifferentTypesAreDistinct()
    {
        var set = new HashSet<Optional<object>>();

        set.Add(Optional.Of(1));
        set.Add(Optional.Of("1"));
        set.Add(Optional.Of(true));
        set.Add(Optional<string>.Empty);
        set.Add(Optional<int>.Empty);

        set.Count.Should().Be(5);
    }
}