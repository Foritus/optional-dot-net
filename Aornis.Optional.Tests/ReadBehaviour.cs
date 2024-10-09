using Aornis.Tests.Types;
using FluentAssertions;
using System;
using Xunit;

namespace Aornis.Tests
{
    public class ReadBehaviour
    {
        [Fact]
        public void ValueReturnsInputValueForReferenceType()
        {
            var inputValue = "hello world";
            var result = Optional.Of(inputValue);

            result.HasValue.Should().BeTrue();
            result.Value.Should().BeSameAs(inputValue);
        }

        [Fact]
        public void ValueReturnsCopyOfInputForValueType()
        {
            var inputValue = new StructWithFields
            {
                foo = 12
            };

            var result = Optional.Of(inputValue);

            result.HasValue.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(inputValue);
        }

        [Fact]
        public void ValueThrowsWhenEmpty()
        {
            Action func = () => Optional<string>.Empty.Value.ToString();

            func.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ValueThrowsWhenConstructedFromANullReferenceType()
        {
            Action func = () => Optional.Of<string>(null).Value.ToString();

            func.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ValueThrowsWhenConstructedFromADefaultValueType()
        {
            Action func = () => Optional.Of(new StructWithFields()).Value.ToString();

            func.Should().Throw<InvalidOperationException>();
        }
    }
}
