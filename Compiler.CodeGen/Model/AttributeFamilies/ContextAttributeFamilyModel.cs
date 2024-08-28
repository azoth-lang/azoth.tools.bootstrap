using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;

[Closed(typeof(InheritedAttributeFamilyModel), typeof(PreviousAttributeFamilyModel))]
public abstract class ContextAttributeFamilyModel : AttributeFamilyModel
{
    protected ContextAttributeFamilyModel(TreeModel tree)
        : base(tree)
    {
    }
}
