using FluentAssertions;
using Microsoft.FSharp.Core;
using Xunit;

namespace Aornis.Tests
{
    public class ImplicitConversionFSharp
    {
        [Fact]
        public void RoundTripReferenceThroughFSharpAndBack()
        {
            var initial = Optional.Of("nanomachines?!");
            
            FSharpValueOption<string> intermediate = initial.ToFsOption();
            intermediate.IsValueSome.Should().BeTrue();

            Optional<string> result = intermediate;
            result.Should().Be(Optional.Of("nanomachines?!"));
        }
        
        [Fact]
        public void RoundTripEmptyReferenceThroughFSharpAndBack()
        {
            var initial = Optional<string>.Empty;
            
            FSharpValueOption<string> intermediate = initial.ToFsOption();
            intermediate.IsValueNone.Should().BeTrue();

            Optional<string> result = intermediate;
            result.Should().Be(Optional<string>.Empty);
        }
        
        [Fact]
        public void RoundTripValueTypeThroughFSharpAndBack()
        {
            var initial = Optional.Of(1337.1337);
            
            FSharpValueOption<double> intermediate = initial.ToFsOption();
            intermediate.IsValueSome.Should().BeTrue();

            Optional<double> result = intermediate;
            result.Should().Be(Optional.Of(1337.1337));
        }
        
        [Fact]
        public void RoundTripEmptyValueTypeThroughFSharpAndBack()
        {
            var initial = Optional<int>.Empty;
            
            FSharpValueOption<int> intermediate = initial.ToFsOption();
            intermediate.IsValueNone.Should().BeTrue();

            Optional<int> result = intermediate;
            result.Should().Be(Optional<int>.Empty);
        }
        
        [Fact]
        public void ConvertsFSharpReferenceOption()
        {
            var fsOpt = FSharpOption<string>.Some("hello world");

            Optional<string> result = fsOpt;

            result.Should().Be(Optional.Of("hello world"));
        }
        
        [Fact]
        public void ConvertsFSharpEmptyReferenceOption()
        {
            var fsOpt = FSharpOption<string>.None;

            Optional<string> result = fsOpt;

            result.Should().Be(Optional<string>.Empty);
        }

        [Fact]
        public void ConvertsFSharpValueOption()
        {
            var fsOpt = FSharpOption<int>.Some(12345);

            Optional<int> result = fsOpt;

            result.Should().Be(Optional.Of(12345));
        }

        [Fact]
        public void ConvertsFSharpEmptyValueOption()
        {
            var fsOpt = FSharpOption<int>.None;

            Optional<int> result = fsOpt;

            result.Should().Be(Optional<int>.Empty);
        }

        [Fact]
        public void ConvertsFSharpValueOptionReferenceType()
        {
            var fsOpt = FSharpValueOption<string>.NewValueSome("hello world 123");

            Optional<string> result = fsOpt;

            result.Should().Be(Optional.Of("hello world 123"));
        }

        [Fact]
        public void ConvertsFSharpValueOptionEmptyReferenceType()
        {
            var fsOpt = FSharpValueOption<string>.ValueNone;

            Optional<string> result = fsOpt;

            result.Should().Be(Optional<string>.Empty);
        }

        [Fact]
        public void ConvertsFSharpValueOptionValueType()
        {
            var fsOpt = FSharpValueOption<int>.NewValueSome(987654);

            Optional<int> result = fsOpt;

            result.Should().Be(Optional.Of(987654));
        }

        [Fact]
        public void ConvertsFSharpValueOptionEmptyValueType()
        {
            var fsOpt = FSharpValueOption<int>.ValueNone;

            Optional<int> result = fsOpt;

            result.Should().Be(Optional<int>.Empty);
        }
    }
}