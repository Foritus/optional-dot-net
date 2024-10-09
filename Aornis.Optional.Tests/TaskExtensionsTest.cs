using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Aornis.Tests
{
    public class TaskExtensionsTest
    {
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