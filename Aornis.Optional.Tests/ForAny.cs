using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Aornis.Tests;

public class ForAny : TestBase
{
    private string _observed = null;
    private void Callback(string x)
    {
        _observed = x;
    }
    
    [Fact]
    public void ReturnsFirstNonEmptyValue()
    {
        var list = new Optional<string>[]
        {
            Optional.Empty,
            "hello",
            "world",
            Optional.Empty
        };

        Optional.ForAny(list, Callback).IfPresent(result =>
            {
                result.Should().Be("hello");
                result.Should().BeSameAs(_observed);
            })
            .IfNotPresent(() => throw new Exception("Expected the first non-empty value, not empty!"));
    }

    [Fact]
    public void ReturnsEmptyForEmptyList()
    {
        var list = new List<Optional<string>>();

        Optional.ForAny(list, Callback).Should().Be(Optional.Empty);
    }

    [Fact]
    public void ReturnsEmptyforListContainingSingleEmpty()
    {
        var list = new List<Optional<string>>
        {
            Optional.Empty
        };

        Optional.ForAny(list, Callback).Should().Be(Optional.Empty);
    }

    [Fact]
    public void ReturnsFirstValueFromFullyPopulatedList()
    {
        var list = new List<Optional<string>>
        {
            "1",
            "2",
            "3",
            "4"
        };

        Optional.ForAny(list, Callback).IfPresent(result =>
            {
                result.Should().Be("1");
                result.Should().Be(_observed);
            })
            .IfNotPresent(() => throw new Exception("Expected the first value from the list from ForAny!"));
    }
}