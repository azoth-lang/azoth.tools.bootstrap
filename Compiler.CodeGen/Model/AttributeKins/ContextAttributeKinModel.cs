using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeKins;

[Closed(typeof(InheritedAttributeKinModel), typeof(PreviousAttributeKinModel))]
public abstract class ContextAttributeKinModel : AttributeKinModel
{
    protected ContextAttributeKinModel(TreeModel tree)
        : base(tree)
    {
    }
}
