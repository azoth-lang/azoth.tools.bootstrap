using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static partial class TypeOperations
{
    /// <summary>
    /// Validates that a type as been assigned.
    /// </summary>
    [DebuggerHidden]
    public static Pseudotype Assigned([NotNull] this Pseudotype? pseudotype)
        => pseudotype ?? throw new InvalidOperationException("Pseudotype not assigned.");

    /// <summary>
    /// Validates that a type as been assigned.
    /// </summary>
    [DebuggerHidden]
    public static Pseudotype Assigned(this IPromise<Pseudotype?> pseudotype)
        => pseudotype.Result.Assigned();


    /// <summary>
    /// Validates that a type as been assigned.
    /// </summary>
    [DebuggerHidden]
    public static DataType Assigned([NotNull] this DataType? type)
        => type ?? throw new InvalidOperationException("Type not assigned.");

    /// <summary>
    /// Validates that a type as been assigned.
    /// </summary>
    [DebuggerHidden]
    public static DataType Assigned(this IPromise<DataType?> type)
        => type.Result.Assigned();

    /// <summary>
    /// Validates that a bare type as been assigned.
    /// </summary>
    [DebuggerHidden]
    public static TBareType Assigned<TBareType>([NotNull] this TBareType? type)
        where TBareType : BareType
        => type ?? throw new InvalidOperationException("Bare type not assigned.");

    /// <summary>
    /// Validates that a type as been assigned.
    /// </summary>
    [DebuggerHidden]
    public static ReturnType Assigned([NotNull] this ReturnType? returnType)
        => returnType ?? throw new InvalidOperationException("ReturnType not assigned.");
}
