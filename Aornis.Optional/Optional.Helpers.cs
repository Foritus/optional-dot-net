using System;
using System.Collections.Generic;
using System.Linq;

namespace Aornis;

/// <summary>
/// Non-generic Optional type that is used to dynamically map between Optional(T) and Optional.Empty via implicit conversions
/// </summary>
public class Optional
{
    /// <summary>
    /// Represents the empty value for all optional types
    /// </summary>
    public static readonly Optional Empty = new Optional();

    private Optional()
    {

    }

    /// <summary>
    /// Determines if the given TValue is equal to its default value
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static bool IsDefault<TValue>(ref TValue? value)
    {
        if (value is ValueType)
        {
            return false;
        }

// Older frameworks don't have nullable annotations on this comparer and so we get spurious compiler warnings
#pragma warning disable CS8604
        return EqualityComparer<TValue>.Default.Equals(value, default);
#pragma warning restore CS8604
    }

    /// <summary>
    /// Returns true if the given values are all present, otherwise false
    /// </summary>
    /// <param name="values">The values to inspect</param>
    /// <returns><see langword="true"/> if all of the given values are present, otherwise <see langword="false"/></returns>
    public static bool All(IEnumerable<IOptional> values)
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

    /// <summary>
    /// Returns true if the given values are all present, otherwise false
    /// </summary>
    /// <param name="values">The values to inspect</param>
    /// <returns><see langword="true"/> if all of the given values are present, otherwise <see langword="false"/></returns>
    public static bool All<T>(params Optional<T>[] values)
    {
        return All(values.Cast<IOptional>());
    }

    /// <summary>
    /// Returns true if the given values are all present, otherwise false (kept for backwards compat)
    /// </summary>
    /// <param name="values">The values to inspect</param>
    /// <returns><see langword="true"/> if all of the given values are present, otherwise <see langword="false"/></returns>
    public static bool All(params IOptional[] values)
    {
        return All(values.Cast<IOptional>());
    }

    /// <summary>
    /// Returns <see langword="true"/> if any of the values are present, otherwise <see langword="false"/>
    /// </summary>
    /// <param name="values">The values to inspect</param>
    /// <returns><see langword="true"/> if any of the values are present, otherwise <see langword="false"/></returns>
    public static bool Any(IEnumerable<IOptional> values)
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

    /// <summary>
    /// Returns <see langword="true"/> if any of the values are present, otherwise <see langword="false"/>
    /// </summary>
    /// <param name="values">The values to inspect</param>
    /// <returns><see langword="true"/> if any of the values are present, otherwise <see langword="false"/></returns>
    public static bool Any<T>(params Optional<T>[] values)
    {
        return Any(values.Cast<IOptional>());
    }

    /// <summary>
    /// Returns <see langword="true"/> if any of the values are present, otherwise <see langword="false"/>
    /// </summary>
    /// <param name="values">The values to inspect</param>
    /// <returns><see langword="true"/> if any of the values are present, otherwise <see langword="false"/></returns>
    public static bool Any(params IOptional[] values)
    {
        return Any(values.Cast<IOptional>());
    }

    /// <summary>
    /// Executes the given collection of callbacks until one returns a non-empty Optional, if a non-empty Optional is encountered it is returned
    /// directly to the caller. Otherwise if none of the callbacks return a populated optional, Empty is returned.
    /// </summary>
    /// <typeparam name="TValue">The type of value inside the optionals that will be created by the callbacks</typeparam>
    /// <param name="callbacks">Callbacks to be executed in sequence to try and provide a non-empty Optional</param>
    /// <returns>A populated value from the first callback that returned a non-empty Optional, otherwise Empty</returns>
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

        return Empty;
    }

    /// <summary>
    /// Wraps the given <typeparamref name="TValue"/> inside of an Optional. Returning Optional.Empty for values which are equivalent to the types default value.
    /// e.g. if given null, this will return Optional.Empty.
    /// </summary>
    /// <typeparam name="TValue">The type of value to wrap in an Optional</typeparam>
    /// <param name="value">The value to wrap</param>
    /// <returns>An optional wrapping the given value</returns>
    public static Optional<TValue> Of<TValue>(TValue? value)
    {
        if (IsDefault(ref value))
        {
            return Empty;
        }

        return new Optional<TValue>(value);
    }

    /// <summary>
    /// Takes the given values and packs them into a collection of Optional values
    /// </summary>
    /// <typeparam name="TValue">The type of values to pack</typeparam>
    /// <param name="values">The values to be packed</param>
    /// <returns>An enumerable of Optional values containing the given TValues</returns>
    public static IEnumerable<Optional<TValue>> Pack<TValue>(params TValue?[]? values)
    {
        if (values == null)
        {
            return new List<Optional<TValue>>();
        }

        var result = new List<Optional<TValue>>();
        foreach (var value in values)
        {
            result.Add(Of(value));
        }
        return result;
    }

    /// <summary>
    /// Given some values, returns either a populated list of those values if they are ALL present, otherwise it returns empty
    /// </summary>
    /// <typeparam name="T">The type of the individual elements</typeparam>
    /// <param name="values">An enumerable collection of values to inspect and repackage</param>
    public static Optional<IEnumerable<T>> UnpackAll<T>(params Optional<T>[] values)
    {
        return UnpackAll((IEnumerable<Optional<T>>)values);
    }

    /// <summary>
    /// Given an enumerable list of values, returns either a populated list of those values if they are ALL present, otherwise it returns empty
    /// </summary>
    /// <typeparam name="T">The type of the individual elements</typeparam>
    /// <param name="values">An enumerable collection of values to inspect and repackage</param>
    /// <returns>A populated list of those values if they are ALL present, otherwise empty</returns>
    public static Optional<IEnumerable<T>> UnpackAll<T>(IEnumerable<Optional<T>> values)
    {
        var result = new List<T>();

        foreach (var value in values)
        {
            if (!value.HasValue)
            {
                return Empty;
            }

            result.Add(value.Value);
        }

        if (result.Count == 0)
        {
            return Empty;
        }

        return result;
    }

    /// <summary>
    /// Given some values, returns either a populated list of those values which are present,
    /// otherwise it returns empty if NO values are present
    /// </summary>
    /// <typeparam name="T">The type of the individual elements</typeparam>
    /// <param name="values">Some values to inspect and repackage</param>
    /// <returns>A populated list of all present values if any are present, otherwise empty</returns>
    public static Optional<IEnumerable<T>> UnpackPartial<T>(params Optional<T>[] values)
    {
        return UnpackPartial((IEnumerable<Optional<T>>)values);
    }

    /// <summary>
    /// Given an enumerable list of values, returns either a populated list of those values which are present,
    /// otherwise it returns empty if NO values are present
    /// </summary>
    /// <typeparam name="T">The type of the individual elements</typeparam>
    /// <param name="values">An enumerable collection of values to inspect and repackage</param>
    /// <returns>A populated list of all present values if any are present, otherwise empty</returns>
    public static Optional<IEnumerable<T>> UnpackPartial<T>(IEnumerable<Optional<T>> values)
    {
        var result = new List<T>();

        foreach (var value in values)
        {
            if (!value.HasValue)
            {
                continue;
            }

            result.Add(value.Value);
        }

        if (result.Count == 0)
        {
            return Empty;
        }

        return result;
    }

    /// <summary>
    /// Given a collection of functions which produce optional values, will call each function in sequence until one returns a non-empty value.
    /// The found value is returned. If no non-empty value is found, empty is returned.
    /// </summary>
    /// <param name="generators">The collection of functions to execute and check for a non-empty return value</param>
    /// <typeparam name="T">The type of value found within the optional sequence</typeparam>
    /// <returns>The first non-empty value returned from the functions in the given collection of values, otherwise empty</returns>
    public static Optional<T> Coalesce<T>(IEnumerable<Func<Optional<T>>> generators)
    {
        foreach (var func in generators)
        {
            var value = func();
            if (!value.HasValue)
            {
                continue;
            }

            return value;
        }

        return Empty;
    }

    /// <summary>
    /// Given a collection of optional values, will execute the given callback for the FIRST non-empty value found within that set of values. The found value is returned. If no non-empty value is found, empty is returned.
    /// </summary>
    /// <param name="values">The collection of values to search for a non-empty value</param>
    /// <param name="callback">The callback that will be executed once if a non-empty value is found</param>
    /// <typeparam name="T">The type of value found within the optional sequence</typeparam>
    /// <returns>The first non-empty value found in the given collection of values, otherwise empty</returns>
    public static Optional<T> ForAny<T>(IEnumerable<Optional<T>> values, Action<T> callback)
    {
        foreach (var value in values)
        {
            if (!value.HasValue)
            {
                continue;
            }

            callback(value.Value);
            return value;
        }

        return Empty;
    }

    /// <inheritdoc cref="Object.ToString"/>
    public override string ToString()
    {
        // The nongeneric version of Optional is always empty
        return "Optional[Empty]";
    }
}