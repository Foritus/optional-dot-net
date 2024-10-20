using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Aornis.Tests;

public class Coalesce : TestBase
{
    [Fact]
    public void ReturnsFirstNonEmptyValue()
    {
        var list = new Func<Optional<string>>[]
        {
            () => Optional.Empty,
            () => "hello",
            () => "world",
            () => Optional.Empty
        };

        Optional.Coalesce(list).IfPresent(result =>
            {
                result.Should().Be("hello");
            })
            .IfNotPresent(() => throw new Exception("Expected the first non-empty value, not empty!"));
    }

    [Fact]
    public void ReturnsEmptyForEmptyList()
    {
        var list = new List<Func<Optional<string>>>();

        Optional.Coalesce(list).Should().Be(Optional.Empty);
    }

    [Fact]
    public void ReturnsEmptyforListContainingSingleEmpty()
    {
        var list = new List<Func<Optional<string>>>
        {
            () => Optional.Empty
        };

        Optional.Coalesce(list).Should().Be(Optional.Empty);
    }

    [Fact]
    public void ReturnsFirstValueFromFullyPopulatedList()
    {
        var list = new List<Func<Optional<string>>>
        {
            () => "1",
            () => "2",
            () => "3",
            () => "4"
        };

        Optional.Coalesce(list).IfPresent(result =>
            {
                result.Should().Be("1");
            })
            .IfNotPresent(() => throw new Exception("Expected the first value from the list from Coalesce!"));
    }
}