using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Aornis.Tests
{
    public class FlatMapTaskExtensionTest
    {
        private Optional<int> baseValue;
        private Optional<int> newValue;

        public FlatMapTaskExtensionTest()
        {
            baseValue = Optional.Of(123);
            newValue = Optional.Of(456);
        }
        
        #region Sync

        [Fact]
        public async Task FlatMapReturnsValueCreatedByFunc()
        {
            var result = await Task.FromResult(baseValue).FlatMapAsync(x => newValue);
            result.Should().Be(newValue);
        }

        [Fact]
        public async Task FlatMapReturnsEmptyWhenFuncReturnsEmpty()
        {
            var result = await Task.FromResult(baseValue).FlatMapAsync(x => Optional<string>.Empty);
            result.Should().Be(Optional<string>.Empty);
        }

        [Fact]
        public async Task FlatMapDoesNotCallFuncWhenValueIsEmpty()
        {
            await Task.FromResult(Optional<string>.Empty).FlatMapAsync(new Func<string, Optional<string>>(x => throw new Exception("This function should not be called")));
        }

        #endregion

        #region Async

        [Fact]
        public async Task FlatMapAsyncReturnsValueCreatedByFunc()
        {
            var result = (await Task.FromResult(baseValue).FlatMapAsync(x => newValue.Map(y => x + y)));
            result.Should().Be(Optional.Of(baseValue.Value + newValue.Value));
        }

        [Fact]
        public async Task FlatMapAsyncReturnsEmptyWhenFuncReturnsEmpty()
        {
            var result = await baseValue.FlatMapAsync(x => Task.FromResult(Optional<string>.Empty));
            result.Should().Be(Optional<string>.Empty);
        }

        [Fact]
        public async Task FlatMapAsyncDoesNotCallFuncWhenValueIsEmpty()
        {
            await Optional<string>.Empty.FlatMapAsync(new Func<string, Task<Optional<string>>>(x => throw new Exception("This function should not be called")));
        }

        [Fact]
        public async Task ReturnsTaskCreatedByAsyncCallback()
        {
            var result = await Task.FromResult(baseValue).FlatMapAsync(async x => await Task.FromResult(newValue));
            result.Should().Be(newValue);
        }
        
        #endregion
    }
}