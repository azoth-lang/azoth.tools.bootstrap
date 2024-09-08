using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

/// <summary>
/// A temporary partial class to make the transition to generated tree nodes possible.
/// </summary>
internal partial class SemanticNode
{
    public virtual IEnumerable<ISemanticNode> Children() => ((ISemanticNode)this).Children();
}
