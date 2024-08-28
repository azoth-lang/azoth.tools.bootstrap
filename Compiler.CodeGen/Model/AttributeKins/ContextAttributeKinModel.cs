using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeKins;

[Closed(typeof(InheritedAttributeKinModel), typeof(PreviousAttributeKinModel))]
public abstract class ContextAttributeKinModel : AttributeKinModel
{
    public TreeModel Tree { get; }
    public abstract string Name { get; }
    public abstract TypeModel Type { get; }

    protected ContextAttributeKinModel(TreeModel tree)
    {
        Tree = tree;
    }
}
