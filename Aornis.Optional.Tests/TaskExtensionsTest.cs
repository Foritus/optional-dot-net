using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Aornis.Tests
{
    public class TaskExtensionsTest
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(128)]
        [InlineData(256)]
        [InlineData(512)]
        [InlineData(1024)]
        [InlineData(2048)]
        [InlineData(4096)]
        public async Task OrElseAsyncChainingExecutesAllCallbacks(int chainLength)
        {
            var t = Task.Run<Optional<int>>(() => Optional.Empty);

            int executedCallbacks = 0;

            for (int i = 1; i <= chainLength; ++i)
            {
                Optional<int> chainResult = (i == chainLength) ? i : Optional.Empty;
                t = t.OrElseAsync(() =>
                {
                    executedCallbacks++;
                    return Task.FromResult(chainResult);
                });
            }

            var result = await t;

            // We should have traversed the entire chain
            executedCallbacks.Should().Be(chainLength);
            result.HasValue.Should().BeTrue();
            result.Value.Should().Be(chainLength);
        }

        [Fact]
        public async Task OrElseAsyncChainingIsLazy()
        {
            int callbacksExecuted = 0;
            
            var t = Task.Run<Optional<int>>(() => Optional.Empty);

            var result = 
                await t.OrElseAsync(() =>
                {
                    callbacksExecuted++;
                    return Optional.Empty;
                })
                .OrElseAsync(() =>
                {
                    callbacksExecuted++;
                    return Task.FromResult(Optional.Of(22));
                })
                .OrElseAsync(() =>
                {
                    // This callback should never be executed as the previous OrElseAsync() returns a value
                    callbacksExecuted++;
                    return Task.FromResult(Optional.Of(3333));
                });

            result.Value.Should().Be(22);
            callbacksExecuted.Should().Be(2);
        }
        
        [Fact]
        public async Task TaskOrElseImmediateValueDoesNotExecuteFuncIfValue()
        {
            var t = new Task<Optional<int>>(() => 1);
            t.Start();

            var result = await t.OrElseAsync(2);

            result.Should().Be(1);
        }
        
        [Fact]
        public async Task TaskOrElseImmediateFuncDoesNotExecuteFuncIfValue()
        {
            var t = new Task<Optional<int>>(() => 1);
            t.Start();

            var result = await t.OrElseAsync(() => 2);

            result.Should().Be(1);
        }
        
        [Fact]
        public async Task TaskOrElseOptionalDoesNotExecuteFuncIfValue()
        {
            var t = new Task<Optional<int>>(() => 1);
            t.Start();

            var result = await t.OrElseAsync(() => Optional.Of(2));

            result.HasValue.Should().BeTrue();
            result.Value.Should().Be(1);
        }

        [Fact]
        public async Task TaskMapAsyncExecuteFuncIfValue()
        {
            var t = new Task<Optional<int>>(() => 123);
            t.Start();

            var result = await t.MapAsync(x =>
            {
                x.Should().Be(123);
                return 256;
            });

            result.HasValue.Should().BeTrue();
            result.Value.Should().Be(256);
        }

        [Fact]
        public async Task TaskMapAsyncTaskExecuteFuncIfValue()
        {
            var t = new Task<Optional<int>>(() => 123);
            t.Start();

            Optional<int> result = await t.MapAsync(x =>
            {
                x.Should().Be(123);
                return Task.FromResult(256);
            });

            result.HasValue.Should().BeTrue();
            result.Value.Should().Be(256);
        }

        [Fact]
        public async Task TaskMapAsyncDoesNotExecuteFuncIfNoValue()
        {
            var t = new Task<Optional<int>>(() => Optional.Empty);
            t.Start();

            var result = await t.MapAsync((Func<int, int>)(x => throw new Exception("This should not be called")));

            result.HasValue.Should().BeFalse();
            Assert.Throws<InvalidOperationException>(() => result.Value);
        }

        [Fact]
        public async Task TaskMapAsyncTaskDoesNotExecuteFuncIfNoValue()
        {
            var t = new Task<Optional<int>>(() => Optional.Empty);
            t.Start();

            var result = await t.MapAsync((Func<int, Task<int>>)(x => throw new Exception("This should not be called")));

            result.HasValue.Should().BeFalse();
            Assert.Throws<InvalidOperationException>(() => result.Value);
        }
    }
}