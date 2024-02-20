using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
    public static DataType Assigned([NotNull] this DataType? type)
        => type ?? throw new InvalidOperationException("Type not assigned.");

    /// <summary>
    /// Validates that a type as been assigned.
    /// </summary>
    [DebuggerHidden]
    public static Return Assigned([NotNull] this Return? returnType)
        => returnType ?? throw new InvalidOperationException("ReturnType not assigned.");
}
