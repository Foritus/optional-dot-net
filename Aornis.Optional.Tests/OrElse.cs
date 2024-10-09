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

        #endregion
    }
}
