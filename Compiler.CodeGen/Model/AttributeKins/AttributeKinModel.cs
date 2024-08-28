using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeKins;

/// <summary>
/// A model for something that applies to all the attributes with a given name in the tree.
/// </summary>
[Closed(typeof(ContextAttributeKinModel))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class AttributeKinModel
{
    public TreeModel Tree { get; }
    public abstract string Name { get; }
    public abstract TypeModel Type { get; }

    protected AttributeKinModel(TreeModel tree)
    {
        Tree = tree;
    }

    public abstract override string ToString();
}
