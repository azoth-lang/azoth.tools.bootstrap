using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

public interface IReadOnlySharingSet : IEnumerable<ISharingVariable>, IEquatable<SharingSet>
{
    bool IsWriteRestricted { get; }
}
