using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

public interface IReadOnlySharingSet : IEnumerable<SharingVariable>, IEquatable<SharingSet>
{
    bool IsWriteRestricted { get; }
}
