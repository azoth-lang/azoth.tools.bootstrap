using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

public interface IAttributeOperations<T, TLock>
    where TLock : struct
{
    [return: NotNullIfNotNull(nameof(location))]
    static abstract T? Read(in T? location, ref TLock syncLock);
    static abstract T WriteFinal(ref T? location, T value, ref TLock syncLock, ref bool cached);
    static abstract bool CompareExchange(
        ref T? location,
        T value,
        T? comparand,
        IEqualityComparer<T> comparer,
        ref TLock syncLock,
        out T? previous);
}
