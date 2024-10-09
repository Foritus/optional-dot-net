using Aornis.Tests.Types;
using FluentAssertions;
using System;
using Xunit;

namespace Aornis.Tests
{
    public class CreatingFromDefaults
    {
        [Fact]
        public void FromNullReferenceType()
        {
            Optional.Of<string>(null).Should().Be(Optional<string>.Empty);
        }

        [Fact]
        public void FromDefaultValueType()
        {
            Optional.Of(default(int)).Should().Be(Optional<int>.Empty);
        }

        [Fact]
        public void FromDefaultValueTypeWithFields()
        {
            Optional.Of(default(StructWithFields)).Should().Be(Optional<StructWithFields>.Empty);
        }
    }
}
