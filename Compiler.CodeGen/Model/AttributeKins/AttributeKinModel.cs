using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.AttributeKins;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeKins;

/// <summary>
/// A model for something that applies to all the attributes with a given name in the tree.
/// </summary>
[Closed(typeof(ContextAttributeKinModel))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class AttributeKinModel
{
    public static AttributeKinModel Create(TreeModel tree, AttributeKinSyntax syntax)
    {
        return syntax switch
        {
            InheritedAttributeKinSyntax syn => new InheritedAttributeKinModel(tree, syn),
            AggregateAttributeKinSyntax syn => throw new NotImplementedException(),
            _ => throw ExhaustiveMatch.Failed(syntax),
        };
    }

    public TreeModel Tree { get; }
    public abstract string Name { get; }
    public abstract TypeModel Type { get; }

    protected AttributeKinModel(TreeModel tree)
    {
        Tree = tree;
    }

    public abstract override string ToString();
}
