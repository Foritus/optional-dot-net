using System;
using System.Collections.Generic;
using System.Text;

namespace Aornis;

/// <summary>
/// Interface for allowing performing common operations on multiple Optional(T) instances with differing T types.
/// </summary>
public interface IOptional
{
    /// <summary>
    /// Returns true if this IOptional has a value, otherwise false
    /// </summary>
    bool HasValue { get; }
}