using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static partial class TypeOperations
{
    [DebuggerHidden]
    public static Pseudotype Known(this Pseudotype? pseudotype)
    {
        if (!pseudotype.Assigned().IsFullyKnown)
            throw new InvalidOperationException($"Type {pseudotype.ToILString()} not fully known.");

        return pseudotype;
    }

    [DebuggerHidden]
    public static Pseudotype Known(this IPromise<Pseudotype> promise)
        => promise.Result.Known();

    [DebuggerHidden]
    public static DataType Known(this DataType? type)
    {
        if (!type.Assigned().IsFullyKnown)
            throw new InvalidOperationException($"Type {type.ToILString()} not fully known.");

        return type;
    }

    [DebuggerHidden]
    public static ReturnType Known(this ReturnType @return)
    {
        if (!@return.Type.IsFullyKnown)
            throw new InvalidOperationException($"Type {@return.ToILString()} not fully known.");

        return @return;
    }

    [DebuggerHidden]
    public static ReturnType Known(this ReturnType? returnType) => returnType.Assigned().Known();

    [DebuggerHidden]
    public static DataType Known(this IPromise<DataType> promise)
        => promise.Result.Known();

    [DebuggerHidden]
    public static BareType Known(this BareType? type)
    {
        if (!type.Assigned().IsFullyKnown)
            throw new InvalidOperationException($"Bare type {type.ToILString()} not fully known.");

        return type;
    }


    [DebuggerHidden]
    public static BareType Known(this IPromise<BareType?> promise)
        => promise.Result.Known();
}
