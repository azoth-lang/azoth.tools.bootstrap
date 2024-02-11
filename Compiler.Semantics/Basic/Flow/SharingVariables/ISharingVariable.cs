using System;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

/// <summary>
/// A "variable" (i.e. an actual variable, parameter, temporary reference, or possible reference)
/// that participates in the sharing analysis.
/// </summary>
public interface ISharingVariable : IEquatable<ISharingVariable>
{
    /// <summary>
    /// Whether this is a declared variable or parameter (as opposed to a temp reference etc.)
    /// </summary>
    bool IsVariableOrParameter { get; }
    CapabilityRestrictions RestrictionsImposed { get; }
    bool SharingIsTracked { get; }
    bool KeepsSetAlive { get; }
}
