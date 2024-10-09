using System;
using System.Runtime.CompilerServices;

namespace Aornis;

internal class ThrowHelper
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void InvalidOperation(string message)
    {
        throw new InvalidOperationException(message);
    }
}