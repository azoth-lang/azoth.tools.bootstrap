using System;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

public interface ISharingVariable : IEquatable<ISharingVariable>
{
    bool IsVariableOrParameter { get; }
    CapabilityRestrictions RestrictionsImposed { get; }
    bool SharingIsTracked { get; }
    bool KeepsSetAlive { get; }
}
