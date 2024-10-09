using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Aornis
{
    public readonly struct Optional<TValue> : IOptional, IEquatable<Optional<TValue>>
    {
        /// <summary>
        /// Represents the 'empty' value for an Optional(TValue) instance
        /// </summary>
        public static readonly Optional<TValue> Empty = new Optional<TValue>(default(TValue), false);

        private readonly TValue value;

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
                    throw new InvalidOperationException($"Unsafe call to {nameof(Value)}() when {HasValue} is false");
                }

                return value;
            }
        }

        /// <summary>
        /// Creates an Optional(TValue) with the given TValue value, if value is equal to the default value for the given TValue (null for reference types, 0-arity constructor for value types)
        /// then this optional will be empty.
        /// </summary>
        /// <param name="value">The value to be stored in this optional type</param>
        public Optional(TValue value) : this()
        {
            this.value = value;
            HasValue = !Optional.IsDefault(ref value);
        }
        
        /// <summary>
        /// Internal workaround for the fact that value types don't play nicely with IsDefault. i.e. if I create an Optional(int) with value 0, this is technically the same as
        /// Optional(int).Empty as default(int) is 0. Doh. So instead allow some manual wiring when creating Optional(int).Empty so explicitly state "hey this is empty"
        /// </summary>
        internal Optional(TValue value, bool hasValue)
        {
            this.value = value;
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

            return mapper(value);
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

            return await mapper(value);
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

            callback(value);
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

            await callback(value);
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

            return Optional.Of(mapper(value));
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

            return Optional.Of(await mapper(value));
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
        /// <param name="fallback">Fallback value if this Optional is empty</param>
        /// <returns>this.Value if this Optional is not empty, otherwise the given fallback value</returns>
        public TValue OrElse(Func<TValue> func)
        {
            return HasValue ? this.Value : func();
        }

        /// <summary>
        /// If this optional is empty, returns the value created by the given function. Otherwise returns the value contained inside this Optional.
        /// </summary>
        /// <param name="fallback">Fallback value if this Optional is empty</param>
        /// <returns>this.Value if this Optional is not empty, otherwise the given fallback value</returns>
        public Optional<TValue> OrElse(Func<Optional<TValue>> func)
        {
            if (HasValue)
            {
                return this;
            }

            return func();
        }

        /// <summary>
        /// If this optional is empty, returns the value created by the given function. Otherwise returns the value contained inside this Optional.
        /// </summary>
        /// <param name="fallback">Fallback value if this Optional is empty</param>
        /// <returns>this.Value if this Optional is not empty, otherwise the given fallback value</returns>
        public async Task<Optional<TValue>> OrElseAsync(Func<Task<TValue>> func)
        {
            if (HasValue)
            {
                return this;
            }

            return Optional.Of(await func());
        }

        /// <summary>
        /// If this optional is empty, returns the value created by the given function. Otherwise returns the value contained inside this Optional.
        /// </summary>
        /// <param name="fallback">Fallback value if this Optional is empty</param>
        /// <returns>this.Value if this Optional is not empty, otherwise the given fallback value</returns>
        public async Task<Optional<TValue>> OrElseAsync(Func<Task<Optional<TValue>>> func)
        {
            if (HasValue)
            {
                return this;
            }

            return await func();
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
        /// <param name="value"></param>
        public static implicit operator Optional<TValue>(TValue value)
        {
            return Optional.Of(value);
        }

        /// <summary>
        /// Implicit operator to convert Optional.Empty into Optional(TValue).Empty
        /// </summary>
        /// <param name="value">The nongeneric Optional to convert</param>
        public static implicit operator Optional<TValue>(Optional value)
        {
            return Empty;
        }

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

        public override int GetHashCode()
        {
            const int HASH_PRIME = 486187739;
            var hashCode = HASH_PRIME;
            if (HasValue)
            {
                hashCode = hashCode * HASH_PRIME + EqualityComparer<TValue>.Default.GetHashCode(value);
            }
            hashCode = hashCode * HASH_PRIME + HasValue.GetHashCode();

            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is Optional wrapped)
            {
                return Equals((Optional<TValue>)wrapped);
            }

            return obj is Optional<TValue> && Equals((Optional<TValue>)obj);
        }

        public bool Equals(Optional<TValue> other)
        {
            return this.HasValue == other.HasValue &&
                   EqualityComparer<TValue>.Default.Equals(value, other.value);
        }

        public static bool operator ==(Optional<TValue> lhs, Optional<TValue> rhs) => lhs.Equals(rhs);
        public static bool operator !=(Optional<TValue> lhs, Optional<TValue> rhs) => !(lhs == rhs);

        public static bool operator ==(Optional<TValue> lhs, Optional rhs) => !lhs.HasValue;
        public static bool operator !=(Optional<TValue> lhs, Optional rhs) => !(lhs == rhs);

        public static bool operator ==(Optional lhs, Optional<TValue> rhs) => !rhs.HasValue;
        public static bool operator !=(Optional lhs, Optional<TValue> rhs) => !(lhs == rhs);

        #endregion
    }
}
