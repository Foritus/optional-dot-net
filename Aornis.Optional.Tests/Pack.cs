using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Aornis.Tests;

public class Pack
{
    [Fact]
    public void NoArgsReturnsEmptyList()
    {
        Optional.Pack<string>().Should().BeEmpty();
    }

    [Fact]
    public void SingleArgReturnsSingletonList()
    {
        Optional.Pack("test").Should().BeEquivalentTo(new List<Optional<string>>
        {
            "test"
        });
    }

    [Fact]
    public void NullArrayReturnsEmptyList()
    {
        Optional.Pack<string>(null).Should().BeEmpty();
    }

    [Fact]
    public void NullStringReturnsListWithEmptyOptional()
    {
        Optional.Pack((string)null).Should().BeEquivalentTo(new List<Optional<string>>
        {
            Optional.Empty
        });
    }

    [Fact]
    public void PackCollectsAllValues()
    {
        Optional.Pack(1, 2, 3, 4, 5, 6).Should().BeEquivalentTo(new List<Optional<int>>
        {
            1, 2, 3, 4, 5, 6
        });
    }
}