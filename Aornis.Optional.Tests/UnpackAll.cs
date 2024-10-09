using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Aornis.Tests
{
    public class UnpackAll : TestBase
    {
        [Fact]
        public void ReturnsForEmptyList()
        {
            var list = new List<Optional<string>>();

            Optional.UnpackAll(list).Should().Be(Optional.Empty);
        }

        [Fact]
        public void ReturnsEmptyforListContainingSingleEmpty()
        {
            var list = new List<Optional<string>>
            {
                Optional.Empty
            };

            Optional.UnpackAll(list).Should().Be(Optional.Empty);
        }

        [Fact]
        public void ReturnsEmptyForMixedListContainingSomeValuesAndSomeEmpties()
        {
            var list = new List<Optional<string>>
            {
                Optional.Empty,
                "hello",
                "world",
                Optional.Empty
            };

            Optional.UnpackAll(list).Should().Be(Optional.Empty);
        }

        [Fact]
        public void ReturnsAllValuesFromFullyPopulatedList()
        {
            var list = new List<Optional<string>>
            {
                "1",
                "2",
                "3",
                "4"
            };

            Optional.UnpackAll(list).IfPresent(result =>
            {
                result.Should().BeEquivalentTo(new List<string> { "1", "2", "3", "4" });
            })
            .IfNotPresent(() => throw new Exception("Expected a populated list from Unpack!"));
        }
    }
}
