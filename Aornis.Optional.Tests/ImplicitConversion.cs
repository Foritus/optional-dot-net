using Aornis.Tests.Types;
using FluentAssertions;
using Xunit;

namespace Aornis.Tests;

public class ImplicitConversion
{
    [Fact]
    public void ConvertsNullReferenceTypeToEmpty()
    {
        Optional<string> result = (string)null;

        result.Should().Be(Optional<string>.Empty);
    }

    [Fact]
    public void ConvertsStringToNonEmptyOptional()
    {
        Optional<string> result = "hello";

        result.HasValue.Should().BeTrue();
        result.Value.Should().Be("hello");
    }

    [Fact]
    public void ConvertsEmptyStructToNonEmptyOptional()
    {
        Optional<StructWithFields> result = new StructWithFields();

        result.HasValue.Should().BeTrue();
    }

    [Fact]
    public void ConvertsIntegerToNonEmptyOptional()
    {
        Optional<int> result = 22;

        result.HasValue.Should().BeTrue();
        result.Value.Should().Be(22);
    }

    [Fact]
    public void ConvertsZeroToNonEmptyOptional()
    {
        Optional<int> result = 0;

        result.HasValue.Should().BeTrue();
        result.Value.Should().Be(0);
    }

    [Fact]
    public void ConvertsZeroFloatToNonEmptyOptional()
    {
        Optional<float> result = 0f;

        result.HasValue.Should().BeTrue();
        result.Value.Should().Be(0f);
    }
}