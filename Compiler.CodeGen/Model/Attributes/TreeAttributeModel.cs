using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

[Closed(typeof(PropertyModel), typeof(ChildPlaceholderModel))]
public abstract class TreeAttributeModel : AttributeModel
{
    public static TreeAttributeModel Create(TreeNodeModel node, TreeAttributeSyntax syntax)
        => syntax switch
        {
            PropertySyntax syn => new PropertyModel(node, syn),
            ChildPlaceholderSyntax syn => new ChildPlaceholderModel(node, syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };
}
