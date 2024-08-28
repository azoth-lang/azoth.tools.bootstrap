using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeSupertypes;

[Closed(typeof(InheritedAttributesModel), typeof(PreviousAttributesModel))]
public abstract class ContextAttributesModel : AttributesModel
{
    public TreeModel Tree { get; }
    public abstract string Name { get; }
    public abstract TypeModel Type { get; }

    protected ContextAttributesModel(TreeModel tree)
    {
        Tree = tree;
    }
}
