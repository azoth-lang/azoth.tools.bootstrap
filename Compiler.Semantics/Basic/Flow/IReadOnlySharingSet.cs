using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

public interface IReadOnlySharingSet : IEnumerable<ISharingVariable>, IEquatable<SharingSetMutable>
{
    bool IsLent { get; }
    CapabilityRestrictions Restrictions { get; }
}
