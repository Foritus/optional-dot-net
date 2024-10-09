using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aornis.Tests
{
    public class OrElse : TestBase
    {
        #region Sync

        [Fact]
        public void OrElseReturnsValueWhenCurrentIsEmpty()
        {
            var expected = "metal gear?!";
            Optional<string>.Empty.OrElse(expected).Should().Be(expected);
        }

        [Fact]
        public void OrElseReturnsValueFromFuncWhenCurrentIsEmpty()
        {
            var expected = "metal gear?!";
            Optional<string>.Empty.OrElse(() => expected).Should().Be(expected);
        }
        
        [Fact]
        public void OrElseCallsFuncWhenValueIsEmpty()
        {
            Optional<string>.Empty.OrElse(() => otherValue).Should().Be(otherValue);
        }

        [Fact]
        public void OrElseDoesNotCallFuncWhenHasValue()
        {
            value.OrElse(() => otherValue).Should().Be(value);
        }

        #endregion

        #region Async

        [Fact]
        public async Task OrElseAsyncCallsFuncWhenValueIsEmpty()
        {
            (await Optional<string>.Empty.OrElseAsync(() => Task.FromResult(otherValue))).Should().Be(otherValue);
        }

        [Fact]
        public async Task OrElseAsyncDoesNotCallFuncWhenHasValue()
        {
            (await value.OrElseAsync(() => Task.FromResult(otherValue))).Should().Be(value);
        }

        [Fact]
        public async Task OrElse_CurrentValueIsSome_RawValueIsSome_ReturnsCurrentValue()
        {
            (await value.OrElseAsync(() => Task.FromResult("hello"))).Should().Be(value);
        }

        [Fact]
        public async Task OrElse_CurrentValueIsSome_RawValueIsNull_ReturnsCurrentValue()
        {
            (await value.OrElseAsync(() => Task.FromResult((string)null))).Should().Be(value);
        }

        [Fact]
        public async Task OrElse_CurrentValueIsNone_RawValueIsSome_ReturnsRawValue()
        {
            string expected = "alpacas";
            (await Optional<string>.Empty.OrElseAsync(() => Task.FromResult(expected))).Should().Be(Optional.Of(expected));
        }

        [Fact]
        public async Task OrElse_CurrentValueIsNone_RawValueIsNull_ReturnsEmpty()
        {
            (await Optional<string>.Empty.OrElseAsync(() => Task.FromResult((string)null))).Should().Be(Optional<string>.Empty);
        }

        #endregion
    }
}
