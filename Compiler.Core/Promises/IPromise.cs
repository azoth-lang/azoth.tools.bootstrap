using System;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Promises;

public interface IPromise<out T>
{
    bool IsFulfilled { get; }
    T Result { get; }
    string ToString();
    public string ToString(Func<T, string> toString)
        => IsFulfilled ? toString(Result) : Promise.PendingString;
}
