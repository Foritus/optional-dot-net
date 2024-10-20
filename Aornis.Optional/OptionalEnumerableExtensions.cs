using System;
using System.Collections.Generic;

namespace Aornis;

public static class OptionalEnumerableExtensions
{
    /// <summary>
    /// Given a collection of functions which produce optional values, will call each function in sequence until we receive up to 
    /// The found value is returned. If no non-empty value is found, empty is returned.
    /// </summary>
    /// <param name="generators">The collection of functions to execute and check for a non-empty return value</param>
    /// <param name="maximumToTake">The maximum number of optional values to take from the sequence of callbacks</param>
    /// <typeparam name="T">The type of value found within the optional sequence</typeparam>
    /// <returns>The first non-empty value returned from the functions in the given collection of values, otherwise empty</returns>
    public static IEnumerator<T> TakePresent<T>(this IEnumerable<Func<Optional<T>>> generators, long maximumToTake)
    {
        long count = 0;
        foreach (var func in generators)
        {
            if (count >= maximumToTake)
            {
                break;
            }

            var value = func();
            if (!value.HasValue)
            {
                continue;
            }

            yield return value.Value;

            count++;
        }
    }

    /// <summary>
    /// Given a collection of optional values, will execute the given callback for the FIRST non-empty value found within that set of values. The found value is returned. If no non-empty value is found, empty is returned.
    /// </summary>
    /// <param name="values">The collection of values to search for a non-empty value</param>
    /// <param name="maximumToTake">The maximum number of non-empty values to take from the collection</param>
    /// <typeparam name="T">The type of value found within the optional sequence</typeparam>
    /// <returns>The first non-empty value found in the given collection of values, otherwise empty</returns>
    public static IEnumerator<T> TakePresent<T>(this IEnumerable<Optional<T>> values, long maximumToTake)
    {
        long count = 0;
        foreach (var value in values)
        {
            if (count >= maximumToTake)
            {
                break;
            }

            if (!value.HasValue)
            {
                continue;
            }

            yield return value.Value;

            count++;
        }
    }
}