using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

/// <summary>
/// A temporary partial class to make the transition to generated tree nodes possible.
/// </summary>
internal partial class SemanticNode
{
    public virtual IEnumerable<ISemanticNode> Children() => ((ISemanticNode)this).Children();
}
