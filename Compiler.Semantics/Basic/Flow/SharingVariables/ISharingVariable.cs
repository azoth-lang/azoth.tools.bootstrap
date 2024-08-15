using System;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

/// <summary>
/// A "variable" (i.e. an actual variable, parameter, temporary reference, or possible reference)
/// that participates in the sharing analysis.
/// </summary>
public interface ISharingVariable : IEquatable<ISharingVariable>
{
    CapabilityRestrictions RestrictionsImposed { get; }
    bool KeepsSetAlive { get; }
}
