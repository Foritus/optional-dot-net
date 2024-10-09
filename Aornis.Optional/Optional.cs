using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;

namespace Aornis;

/// <summary>
/// Represents a potential value of type TValue.
/// </summary>
/// <typeparam name="TValue">The type of value that may be contained within this instance</typeparam>
public readonly struct Optional<TValue> : IOptional, IEquatable<Optional<TValue>>
{
    /// <summary>
    /// Represents the 'empty' value for an Optional(TValue) instance
    /// </summary>
    public static readonly Optional<TValue> Empty = new(value: default, hasValue: false);

    private readonly TValue? _value;

    /// <summary>
    /// Returns true if this Optional contains a value, otherwise false
    /// </summary>
    public bool HasValue { get; }

    /// <summary>
    /// Returns the value held inside this Optional, or throws InvalidOperationException if this Optional does not contain a value
    /// </summary>
    public TValue Value
    {
        get
        {
            if (!HasValue)
            {
                ThrowHelper.InvalidOperation($"Unsafe call to {nameof(Value)}() when {nameof(HasValue)} is false");
            }

            return _value!;
        }
    }

    /// <summary>
    /// Creates an Optional(TValue) with the given TValue value, if value is equal to the default value for the given TValue.
    /// HasValue=false when value=null for reference types, for value types HasValue is always true.
    /// then this optional will be empty.
    /// </summary>
    /// <param name="value">The value to be stored in this optional type</param>
    public Optional(TValue? value) : this()
    {
        _value = value;
        HasValue = !Optional.IsDefault(ref value);
    }

    /// <summary>
    /// Internal workaround for the fact that value types don't play nicely with IsDefault. i.e. if I create an Optional(int) with value 0, this is technically the same as
    /// Optional(int).Empty as default(int) is 0. Doh. So instead allow some manual wiring when creating Optional(int).Empty so explicitly state "hey this is empty"
    /// </summary>
    private Optional(TValue? value, bool hasValue)
    {
        _value = value;
        HasValue = hasValue;
    }

    /// <summary>
    /// Passes the value contained within this Optional and returns the Optional value created by the given mapping function.
    /// If this Optional is empty, the function will NOT be called.
    /// </summary>
    /// <typeparam name="TNewValue">The type of the new value returned by the mapping function</typeparam>
    /// <param name="mapper">The function to map this Optional(TValue) to an Optional(TNewValue)</param>
    /// <returns>The value returned by mapper, or Empty</returns>
    public Optional<TNewValue> FlatMap<TNewValue>(Func<TValue, Optional<TNewValue>> mapper)
    {
        if (!HasValue)
        {
            return Optional.Empty;
        }

        return mapper(_value!);
    }

    /// <summary>
    /// Passes the value contained within this Optional and returns the Optional value created by the given mapping function.
    /// If this Optional is empty, the function will NOT be called.
    /// </summary>
    /// <typeparam name="TNewValue">The type of the new value returned by the mapping function</typeparam>
    /// <param name="mapper">The function to map this Optional(TValue) to an Optional(TNewValue)</param>
    /// <returns>The value returned by mapper, or Empty</returns>
    public async Task<Optional<TNewValue>> FlatMapAsync<TNewValue>(Func<TValue, Task<Optional<TNewValue>>> mapper)
    {
        if (!HasValue)
        {
            return Optional.Empty;
        }

        return await mapper(_value!);
    }

    /// <summary>
    /// Calls the given function only if this Optional DOES contain a value
    /// </summary>
    /// <param name="callback">The function to call if this Optional contains a value</param>
    /// <returns>This Optional</returns>
    public Optional<TValue> IfPresent(Action<TValue> callback)
    {
        if (!HasValue)
        {
            return this;
        }

        callback(_value!);
        return this;
    }

    /// <summary>
    /// Calls the given function only if this Optional DOES contain a value
    /// </summary>
    /// <param name="callback">The function to call if this Optional contains a value</param>
    /// <returns>This Optional</returns>
    public async Task<Optional<TValue>> IfPresentAsync(Func<TValue, Task> callback)
    {
        if (!HasValue)
        {
            return this;
        }

        await callback(_value!);
        return this;
    }

    /// <summary>
    /// Calls the given function only if this Optional does NOT contain a value
    /// </summary>
    /// <param name="callback">The function to call if this Optional is empty</param>
    /// <returns>This Optional</returns>
    public Optional<TValue> IfNotPresent(Action callback)
    {
        if (HasValue)
        {
            return this;
        }

        callback();
        return this;
    }

    /// <summary>
    /// Calls the given function only if this Optional does NOT contain a value
    /// </summary>
    /// <param name="callback">The function to call if this Optional is empty</param>
    /// <returns>This Optional</returns>
    public async Task<Optional<TValue>> IfNotPresentAsync(Func<Task> callback)
    {
        if (HasValue)
        {
            return this;
        }

        await callback();
        return this;
    }

    /// <summary>
    /// If this optional contains a value, calls the given mapping function and passes the value inside this Optional
    /// </summary>
    /// <typeparam name="TNewValue">The type of the value created by the mapping function</typeparam>
    /// <param name="mapper">The mapping function to map this Optional(TValue) into an Optional(TNewValue)</param>
    /// <returns>The newly mapped value, or Empty if this Optional is empty</returns>
    public Optional<TNewValue> Map<TNewValue>(Func<TValue, TNewValue> mapper)
    {
        if (!HasValue)
        {
            return Optional.Empty;
        }

        return Optional.Of(mapper(_value!));
    }

    /// <summary>
    /// If this optional contains a value, calls the given mapping function and passes the value inside this Optional
    /// </summary>
    /// <typeparam name="TNewValue">The type of the value created by the mapping function</typeparam>
    /// <param name="mapper">The mapping function to map this Optional(TValue) into an Optional(TNewValue)</param>
    /// <returns>The newly mapped value, or Empty if this Optional is empty</returns>
    public async Task<Optional<TNewValue>> MapAsync<TNewValue>(Func<TValue, Task<TNewValue>> mapper)
    {
        if (!HasValue)
        {
            return Optional.Empty;
        }

        return Optional.Of(await mapper(_value!));
    }

    /// <summary>
    /// If this optional is empty, returns the given TValue. Otherwise returns the value contained inside this Optional.
    /// </summary>
    /// <param name="fallback">Fallback value if this Optional is empty</param>
    /// <returns>this.Value if this Optional is not empty, otherwise the given fallback value</returns>
    public TValue OrElse(TValue fallback)
    {
        return HasValue ? this.Value : fallback;
    }

    /// <summary>
    /// If this optional is empty, returns the value created by the given function. Otherwise returns the value contained inside this Optional.
    /// </summary>
    /// <param name="fallback">Fallback value generator if this Optional is empty</param>
    /// <returns>this.Value if this Optional is not empty, otherwise the given fallback value</returns>
    public TValue OrElse(Func<TValue> fallback)
    {
        return HasValue ? this.Value : fallback();
    }

    /// <summary>
    /// If this optional is empty, returns the value created by the given function. Otherwise returns the value contained inside this Optional.
    /// </summary>
    /// <param name="fallback">Fallback value generator if this Optional is empty</param>
    /// <returns>this.Value if this Optional is not empty, otherwise the given fallback value</returns>
    public Optional<TValue> OrElse(Func<Optional<TValue>> fallback)
    {
        if (HasValue)
        {
            return this;
        }

        return fallback();
    }

    /// <summary>
    /// If this optional is empty, throws the given exception while preserving its original stack trace.
    /// </summary>
    /// <returns>this.Value if this Optional is not empty, otherwise throws</returns>
    public TValue OrElseThrow(Exception exception)
    {
        return OrElseThrow(() => exception);
    }

    /// <summary>
    /// If this optional is empty, calls the given function and throws the returned exception while preserving its original stack trace.
    /// </summary>
    /// <returns>this.Value if this Optional is not empty, otherwise throws</returns>
    public TValue OrElseThrow(Func<Exception> makeException)
    {
        if (!HasValue)
        {
            Exception ex = makeException();
            ExceptionDispatchInfo.Capture(ex).Throw();
            // Boilerplate required to make the compiler happy
            return Value;
        }

        return Value;
    }

    /// <summary>
    /// If this optional is empty, returns the value created by the given function. Otherwise returns the value contained inside this Optional.
    /// </summary>
    /// <param name="fallback">Fallback value generator if this Optional is empty</param>
    /// <returns>this.Value if this Optional is not empty, otherwise the given fallback value</returns>
    public async Task<TValue> OrElseAsync(Func<Task<TValue>> fallback)
    {
        if (HasValue)
        {
            return Value;
        }

        return await fallback();
    }

    /// <summary>
    /// If this optional is empty, returns the value created by the given function. Otherwise returns the value contained inside this Optional.
    /// </summary>
    /// <param name="fallback">Fallback value generator if this Optional is empty</param>
    /// <returns>this.Value if this Optional is not empty, otherwise the given fallback value</returns>
    public async Task<Optional<TValue>> OrElseAsync(Func<Task<Optional<TValue>>> fallback)
    {
        if (HasValue)
        {
            return this;
        }

        return await fallback();
    }

    /// <summary>
    /// Throws the given exception if the current optional is empty, otherwise returns the current optional for further processing
    /// </summary>
    /// <param name="ex">The exception to throw if the current value is empty</param>
    public Optional<TValue> ThrowIfEmpty(Exception ex)
    {
        return ThrowIfEmpty(() => ex);
    }

    /// <summary>
    /// Calls the given function and throws the exception it returns if the current optional is empty, otherwise returns the current
    /// optional for further processing
    /// </summary>
    /// <param name="makeException">Function to only call if the current optional is empty</param>
    /// <returns>The current optional value</returns>
    public Optional<TValue> ThrowIfEmpty(Func<Exception> makeException)
    {
        if (HasValue)
        {
            return this;
        }

        Exception ex = makeException();
        ExceptionDispatchInfo.Capture(ex).Throw();
        // Boilerplate required to make the compiler happy
        return this;
    }

    /// <summary>
    /// Throws the exception created by the given function if this Optional is empty
    /// </summary>
    /// <param name="makeException">Generates an exception to be thrown</param>
    /// <returns>This instance if it has a value, otherwise throws</returns>
    public async Task<Optional<TValue>> ThrowIfEmptyAsync(Func<Task<Exception>> makeException)
    {
        if (HasValue)
        {
            return this;
        }

        Exception ex = await makeException();
        ExceptionDispatchInfo.Capture(ex).Throw();
        // Boilerplate required to make the compiler happy
        return this;
    }

    /// <summary>
    /// Converts the given TValue into an Optional(TValue)
    /// </summary>
    public static implicit operator Optional<TValue>(TValue? value)
    {
        return Optional.Of(value);
    }

    /// <summary>
    /// Implicit operator to convert Optional.Empty into Optional(TValue).Empty
    /// </summary>
    /// <param name="_">Ignored</param>
    public static implicit operator Optional<TValue>(Optional? _)
    {
        return Empty;
    }

    /// <summary>
    /// Provides a handy implicit conversion from F#'s option type to our C#-friendly Option type (F#'s option type is really obnoxious without
    /// all of the syntactic sugar that F# provides)
    /// </summary>
    /// <param name="fsOption">The F# option to convert</param>
    /// <returns>An Optional.net equivalent of the F# option</returns>
    public static implicit operator Optional<TValue>(FSharpOption<TValue>? fsOption)
    {
        if (fsOption == null || fsOption.Equals(FSharpOption<TValue>.None))
        {
            return Empty;
        }

        return Optional.Of(fsOption.Value);
    }

    /// <summary>
    /// Provides a handy implicit conversion from F#'s option type to our C#-friendly Option type. The F# ValueOption is less poorly designed than FSharpOption
    /// but a conversion is provided here for completeness.
    /// </summary>
    /// <param name="fsOption">The F# option to convert</param>
    /// <returns>An Optional.net equivalent of the F# option</returns>
    public static implicit operator Optional<TValue>(FSharpValueOption<TValue> fsOption)
    {
        return fsOption.IsValueNone ? Empty : Optional.Of(fsOption.Value);
    }

    /// <summary>
    /// Provides a simple explicit way to map back from an Optional.net option to an F#-friendly option. Given F# does not allow implicit conversions,
    /// this is the neatest way to achieve this.
    /// </summary>
    /// <returns>An F# option that is equivalent to this optional instance</returns>
    public FSharpValueOption<TValue> ToFsOption()
    {
        return HasValue ? FSharpValueOption<TValue>.NewValueSome(Value) : FSharpValueOption<TValue>.ValueNone;
    }

    /// <inheritdoc cref="Object.ToString" />
    public override string ToString()
    {
        if (HasValue)
        {
            return $"Optional[{Value}]";
        }
        else
        {
            return "Optional[Empty]";
        }
    }

    #region Equality Operators

    /// <inheritdoc cref="Object.GetHashCode" />
    public override int GetHashCode()
    {
        const int hashPrime = 486187739;
        var hashCode = hashPrime;
        if (HasValue)
        {
            hashCode = hashCode * hashPrime + EqualityComparer<TValue>.Default.GetHashCode(_value!);
        }

        unchecked
        {
            hashCode = hashCode * hashPrime + HasValue.GetHashCode();
        }

        return hashCode;
    }

    /// <inheritdoc cref="object.Equals(object)" />
    public override bool Equals(object? obj)
    {
        if (obj is Optional wrapped)
        {
            return Equals((Optional<TValue>)wrapped);
        }

        return obj is Optional<TValue> optional && Equals(optional);
    }

    /// <inheritdoc cref="object.Equals(object)"/>
    public bool Equals(Optional<TValue> other)
    {
        return this.HasValue == other.HasValue &&
               EqualityComparer<TValue>.Default.Equals(_value!, other._value!);
    }

    /// <summary>
    /// Determines if the given instances are equal to each other. If both optionals have a value,
    /// the default equality comparator for the given types will be called to determine if they are equal.
    /// </summary>
    /// <param name="lhs">Left hand Optional to check</param>
    /// <param name="rhs">Right hand Optional to check</param>
    /// <returns>true if both optional values are equal or both are Optional.Empty, otherwise false</returns>
    public static bool operator ==(Optional<TValue> lhs, Optional<TValue> rhs) => lhs.Equals(rhs);

    /// <summary>
    /// Determines if the given instances are not equal to each other. If both optionals have a value,
    /// the default equality comparator for the given types will be called to determine if they are not equal.
    /// </summary>
    /// <param name="lhs">Left hand Optional to check</param>
    /// <param name="rhs">Right hand Optional to check</param>
    /// <returns>false if both optional values are equal or both are Optional.Empty, otherwise true</returns>
    public static bool operator !=(Optional<TValue> lhs, Optional<TValue> rhs) => !(lhs == rhs);

    /// <summary>
    /// Compares the given generic Optional against the non-generic Optional.Empty implementation.
    /// This will always return false if the non-generic operand has a value.
    /// </summary>
    /// <param name="lhs">The generic Optional to check</param>
    /// <param name="rhs">The non-generic Optional to check</param>
    /// <returns>true if they are both empty, otherwise false</returns>
    public static bool operator ==(Optional<TValue> lhs, Optional rhs) => !lhs.HasValue;

    /// <summary>
    /// Compares the given generic Optional against the non-generic Optional.Empty implementation.
    /// This will always return true if the non-generic operand has a value.
    /// </summary>
    /// <param name="lhs">The generic Optional to check</param>
    /// <param name="rhs">The non-generic Optional to check</param>
    /// <returns>false if they are both empty, otherwise true</returns>
    public static bool operator !=(Optional<TValue> lhs, Optional rhs) => !(lhs == rhs);

    /// <summary>
    /// Compares the given generic Optional against the non-generic Optional.Empty implementation.
    /// This will always return false if the non-generic operand has a value.
    /// </summary>
    /// <param name="lhs">The non-generic Optional to check</param>
    /// <param name="rhs">The generic Optional to check</param>
    /// <returns>true if they are both empty, otherwise false</returns>
    public static bool operator ==(Optional lhs, Optional<TValue> rhs) => !rhs.HasValue;

    /// <summary>
    /// Compares the given generic Optional against the non-generic Optional.Empty implementation.
    /// This will always return true if the non-generic operand has a value.
    /// </summary>
    /// <param name="lhs">The generic Optional to check</param>
    /// <param name="rhs">The non-generic Optional to check</param>
    /// <returns>false if they are both empty, otherwise true</returns>
    public static bool operator !=(Optional lhs, Optional<TValue> rhs) => !(lhs == rhs);

    #endregion
}