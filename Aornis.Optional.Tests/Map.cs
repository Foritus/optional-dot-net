using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace Aornis.Tests
{
    public class Map
    {
        private const string initialValue = "bees";
        private readonly Optional<string> value;

        public Map()
        {
            value = Optional.Of(initialValue);
        }

        #region Sync
        [Fact]
        public void MapFuncIsCalledWhenHasValue()
        {
            bool wasCalled = false;

            value.Map(x =>
            {
                wasCalled = true;
                return x;
            });

            wasCalled.Should().BeTrue();
        }

        [Fact]
        public void MapPassesValueToFunc()
        {
            value.Map(x =>
            {
                x.Should().Be(initialValue);
                return x;
            });
        }

        [Fact]
        public void MapReturnsValueReturnedByFunc()
        {
            var newValue = "newvalue4324242";
            var result = value.Map(x => newValue);

            result.HasValue.Should().BeTrue();
            result.Value.Should().Be(newValue);
        }
        #endregion

        #region Async

        [Fact]
        public async Task MapAsyncFuncIsCalledWhenHasValue()
        {
            bool wasCalled = false;

            await value.MapAsync(x =>
            {
                wasCalled = true;
                return Task.FromResult(x);
            });

            wasCalled.Should().BeTrue();
        }

        [Fact]
        public async Task MapAsyncPassesValueToFunc()
        {
            await value.MapAsync(x =>
            {
                x.Should().Be(initialValue);
                return Task.FromResult(x);
            });
        }

        [Fact]
        public async Task MapAsyncReturnsValueReturnedByFunc()
        {
            var newValue = "newvalue4324242";
            var result = await value.MapAsync(x => Task.FromResult(newValue));

            result.HasValue.Should().BeTrue();
            result.Value.Should().Be(newValue);
        }

        #endregion
    }
}
