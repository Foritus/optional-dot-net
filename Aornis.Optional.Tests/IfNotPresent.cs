using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Aornis.Tests
{
    public class IfNotPresent
    {
        private const string initialValue = "hello";
        private readonly Optional<string> value;

        public IfNotPresent()
        {
            value = Optional.Of(initialValue);
        }

        #region Sync

        [Fact]
        public void IfNotPresentDoesNotCallFuncWhenHasValue()
        {
            value.IfNotPresent(() => throw new Exception("This function should not be called"));
        }

        [Fact]
        public void IfNotPresentCallsFuncWhenValueIsEmpty()
        {
            bool wasCalled = false;
            Optional<string>.Empty.IfNotPresent(() => wasCalled = true);

            wasCalled.Should().BeTrue();
        }

        [Fact]
        public void IfNotPresentReturnsOptional()
        {
            value.IfNotPresent(() => Console.WriteLine("Hello")).Should().Be(value);
        }

        #endregion

        #region Async

        [Fact]
        public async Task IfNotPresentAsyncDoesNotCallFuncWhenHasValue()
        {
            bool wasCalled = false;
            await Optional.Of("foo").IfNotPresentAsync(() =>
            {
                wasCalled = true;
                return Task.CompletedTask;
            });
            wasCalled.Should().BeFalse();
        }

        [Fact]
        public async Task IfNotPresentAsyncCallsFuncWhenEmpty()
        {
            bool wasCalled = false;
            await Optional<string>.Empty.IfNotPresentAsync(() =>
            {
                wasCalled = true;
                return Task.CompletedTask;
            });
            wasCalled.Should().BeTrue();
        }

        [Fact]
        public async Task IfNotPresentAsyncReturnsOptional()
        {
            (await value.IfNotPresentAsync(() => Task.CompletedTask)).Should().Be(value);
        }

        #endregion
    }
}
