using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Aornis.Tests
{
    public class UnpackPartial : TestBase
    {
        [Fact]
        public void ReturnsPresentValuesForMixedListContainingSomeValuesAndSomeEmpties()
        {
            var list = new List<Optional<string>>
            {
                Optional.Empty,
                "hello",
                "world",
                Optional.Empty
            };

            Optional.UnpackPartial(list).IfPresent(result =>
            {
                result.Should().BeEquivalentTo(new List<string> { "hello", "world" });
            })
            .IfNotPresent(() => throw new Exception("Expected a populated list, not empty!"));
        }

        [Fact]
        public void ReturnsEmptyForEmptyList()
        {
            var list = new List<Optional<string>>();

            Optional.UnpackPartial(list).Should().Be(Optional.Empty);
        }

        [Fact]
        public void ReturnsEmptyforListContainingSingleEmpty()
        {
            var list = new List<Optional<string>>
            {
                Optional.Empty
            };

            Optional.UnpackPartial(list).Should().Be(Optional.Empty);
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

            Optional.UnpackPartial(list).IfPresent(result =>
            {
                result.Should().BeEquivalentTo(new List<string> { "1", "2", "3", "4" });
            })
            .IfNotPresent(() => throw new Exception("Expected a populated list from Unpack!"));
        }
    }
}
