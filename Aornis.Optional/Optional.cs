using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aornis
{
    public struct Optional<TValue> : IOptional, IEquatable<Optional<TValue>>
    {
        public static readonly Optional<TValue> Empty = new Optional<TValue>(default(TValue));

        private readonly TValue value;

        public bool HasValue { get; }

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

        public Optional(TValue value) : this()
        {
            this.value = value;
            HasValue = !Optional.IsDefault(value);
        }

        public Optional<TNewValue> FlatMap<TNewValue>(Func<TValue, Optional<TNewValue>> mapper)
        {
            if (!HasValue)
            {
                return Optional.Empty;
            }

            return mapper(value);
        }

        public async Task<Optional<TNewValue>> FlatMapAsync<TNewValue>(Func<TValue, Task<Optional<TNewValue>>> mapper)
        {
            if (!HasValue)
            {
                return Optional.Empty;
            }

            return await mapper(value);
        }

        public Optional<TValue> IfPresent(Action<TValue> callback)
        {
            if (!HasValue)
            {
                return this;
            }

            callback(value);
            return this;
        }

        public async Task<Optional<TValue>> IfPresentAsync(Func<TValue, Task> callback)
        {
            if (!HasValue)
            {
                return this;
            }

            await callback(value);
            return this;
        }

        public Optional<TValue> IfNotPresent(Action callback)
        {
            if (HasValue)
            {
                return this;
            }

            callback();
            return this;
        }

        public async Task<Optional<TValue>> IfNotPresentAsync(Func<Task> callback)
        {
            if (HasValue)
            {
                return this;
            }

            await callback();
            return this;
        }

        public Optional<TNewValue> Map<TNewValue>(Func<TValue, TNewValue> mapper)
        {
            if (!HasValue)
            {
                return Optional.Empty;
            }

            return Optional.Of(mapper(value));
        }

        public async Task<Optional<TNewValue>> MapAsync<TNewValue>(Func<TValue, Task<TNewValue>> mapper)
        {
            if (!HasValue)
            {
                return Optional.Empty;
            }

            return Optional.Of(await mapper(value));
        }

        public Optional<TValue> OrElse(TValue fallback)
        {
            if (HasValue)
            {
                return this;
            }

            return Optional.Of(fallback);
        }

        public Optional<TValue> OrElse(Func<TValue> func)
        {
            if (HasValue)
            {
                return this;
            }

            return Optional.Of(func());
        }

        public async Task<Optional<TValue>> OrElseAsync(Func<Task<TValue>> func)
        {
            if (HasValue)
            {
                return this;
            }

            return Optional.Of(await func());
        }

        public Optional<TValue> OrElse(Func<Optional<TValue>> func)
        {
            if (HasValue)
            {
                return this;
            }

            return func();
        }

        public async Task<Optional<TValue>> OrElseAsync(Func<Task<Optional<TValue>>> func)
        {
            if (HasValue)
            {
                return this;
            }

            return await func();
        }

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
    }
}
