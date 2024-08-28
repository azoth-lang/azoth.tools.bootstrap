using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.AttributeFamilies;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;

/// <summary>
/// A model for something that applies to all the attributes with a given name in the tree.
/// </summary>
[Closed(typeof(ContextAttributeFamilyModel), typeof(AggregateAttributeFamilyModel))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class AttributeFamilyModel
{
    public static AttributeFamilyModel Create(TreeModel tree, AttributeFamilySyntax syntax)
    {
        return syntax switch
        {
            InheritedAttributeFamilySyntax syn => new InheritedAttributeFamilyModel(tree, syn),
            AggregateAttributeFamilySyntax syn => new AggregateAttributeFamilyModel(tree, syn),
            _ => throw ExhaustiveMatch.Failed(syntax),
        };
    }

    public TreeModel Tree { get; }
    public abstract string Name { get; }
    public abstract TypeModel Type { get; }

    protected AttributeFamilyModel(TreeModel tree)
    {
        Tree = tree;
    }

    public abstract override string ToString();
}
