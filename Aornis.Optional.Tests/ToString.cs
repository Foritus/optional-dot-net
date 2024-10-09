using System;
using FluentAssertions;
using Xunit;

namespace Aornis.Tests;

public class ToString
{
    [Fact]
    public void ToStringPresent()
    {
        var x = Optional.Of("cheese");

        x.ToString().Should().Be("Optional[cheese]");
    }

    [Fact]
    public void ToStringEmpty()
    {
        Optional<string>.Empty.ToString().Should().Be("Optional[Empty]");
    }

    [Fact]
    public void ToStringAnonymousType()
    {
        var x = Optional.Of(new
        {
            Hello = "world",
            Date = DateTime.MinValue,
            Cheese = (string)null
        });

        x.ToString().Should().Be("Optional[{ Hello = world, Date = 01/01/0001 00:00:00, Cheese =  }]");
    }
}