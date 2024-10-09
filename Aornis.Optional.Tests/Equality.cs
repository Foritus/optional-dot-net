using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Aornis.Tests
{
    public class Equality : TestBase
    {
        [Fact]
        public void ValueIsNotEqualToNonGenericEmpty()
        {
            (value == Optional.Empty).Should().BeFalse();
            (Optional.Empty == value).Should().BeFalse();
        }

        [Fact]
        public void ValueIsEqualToItself()
        {
#pragma warning disable CS1718 // Comparison made to same variable
            (value == value).Should().BeTrue();
#pragma warning restore CS1718 // Comparison made to same variable
        }

        [Fact]
        public void ValueIsEqualToOtherValueIsFalse()
        {
            (value == otherValue).Should().BeFalse();
            (otherValue == value).Should().BeFalse();
        }

        [Fact]
        public void ValueIsNotEqualToOtherValueIsTrue()
        {
            (value != otherValue).Should().BeTrue();
            (otherValue != value).Should().BeTrue();
        }

        [Fact]
        public void ValueIsNotEqualToEmpty()
        {
            (Optional.Of("hello") != Optional<string>.Empty).Should().BeTrue();
        }

        [Fact]
        public void EmptyIsEqualToNonGenericEmpty()
        {
            (Optional.Empty == Optional<string>.Empty).Should().BeTrue();
        }
    }
}
