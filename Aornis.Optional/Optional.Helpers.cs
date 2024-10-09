using System;
using System.Collections.Generic;
using System.Text;

namespace Aornis
{
    public class Optional
    {
        public static readonly Optional Empty = new Optional();

        private Optional()
        {

        }

        public static bool All(params IOptional[] values)
        {
            foreach (var val in values)
            {
                if (!val.HasValue)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Any(params IOptional[] values)
        {
            foreach (var val in values)
            {
                if (val.HasValue)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsDefault<TValue>(TValue value)
        {
            if (value is ValueType)
            {
                return false;
            }

            return EqualityComparer<TValue>.Default.Equals(value, default(TValue));
        }

        public static Optional<TValue> Get<TValue>(params Func<Optional<TValue>>[] callbacks)
        {
            foreach (var cb in callbacks)
            {
                Optional<TValue> result = cb();

                if (result.HasValue)
                {
                    return result;
                }
            }

            return Optional.Empty;
        }

        public static Optional<TValue> Of<TValue>(TValue value)
        {
            if (IsDefault(value))
            {
                return Optional.Empty;
            }

            return new Optional<TValue>(value);
        }
    }
}
