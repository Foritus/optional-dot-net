using FluentAssertions;
using System;
using Xunit;

namespace Aornis.Tests;

public class First : TestBase
{
    [Fact]
    public void Value_ReturnsFirstNonEmptyValue()
    {
        var list = new Optional<string>[]
        {
            Optional.Empty,
            "hello",
            "world",
            Optional.Empty
        };

        Optional.First(list).IfPresent(result =>
            {
                result.Should().Be("hello");
            })
            .IfNotPresent(() => throw new Exception("Expected the first non-empty value, not empty!"));
    }

    [Fact]
    public void Value_ReturnsEmptyForEmptyList()
    {
        var list = new Optional<string>[0];

        Optional.First(list).Should().Be(Optional.Empty);
    }

    [Fact]
    public void Value_ReturnsEmptyForListContainingSingleEmpty()
    {
        var list = new Optional<string>[]
        {
            Optional.Empty
        };

        Optional.First(list).Should().Be(Optional.Empty);
    }

    [Fact]
    public void Value_ReturnsFirstValueFromFullyPopulatedList()
    {
        var list = new Optional<string>[]
        {
            "1",
            "2",
            "3",
            "4"
        };

        Optional.First(list).IfPresent(result =>
            {
                result.Should().Be("1");
            })
            .IfNotPresent(() => throw new Exception("Expected the first value from the list from First!"));
    }
    
    
    [Fact]
    public void Callback_ReturnsFirstNonEmptyValue()
    {
        var list = new Func<Optional<string>>[]
        {
            () => Optional.Empty,
            () => "hello",
            () => "world",
            () => Optional.Empty
        };

        Optional.First(list).IfPresent(result =>
            {
                result.Should().Be("hello");
            })
            .IfNotPresent(() => throw new Exception("Expected the first non-empty value, not empty!"));
    }

    [Fact]
    public void Callback_ReturnsEmptyForEmptyList()
    {
        var list = new Func<Optional<string>>[0];

        Optional.First(list).Should().Be(Optional.Empty);
    }

    [Fact]
    public void ReturnsEmptyforListContainingSingleEmpty()
    {
        var list = new Func<Optional<string>>[]
        {
            () => Optional.Empty
        };

        Optional.First(list).Should().Be(Optional.Empty);
    }

    [Fact]
    public void ReturnsFirstValueFromFullyPopulatedList()
    {
        var list = new Func<Optional<string>>[]
        {
            () => "1",
            () => "2",
            () => "3",
            () => "4"
        };

        Optional.First(list).IfPresent(result =>
            {
                result.Should().Be("1");
            })
            .IfNotPresent(() => throw new Exception("Expected the first value from the list from Coalesce!"));
    }
}