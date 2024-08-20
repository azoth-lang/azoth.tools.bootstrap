using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeSupertypes;

[Closed(typeof(InheritedAttributeSupertypeModel), typeof(PreviousAttributeSupertypeModel))]
public abstract class AttributeSupertypeModel
{
    public TreeModel Tree { get; }
    public abstract string Name { get; }
    public abstract TypeModel Type { get; }

    protected AttributeSupertypeModel(TreeModel tree)
    {
        Tree = tree;
    }
}
