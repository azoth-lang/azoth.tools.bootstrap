using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

[Closed(typeof(PropertyModel), typeof(PlaceholderModel))]
public abstract class TreeAttributeModel : AttributeModel
{
    public static TreeAttributeModel Create(TreeNodeModel node, TreeAttributeSyntax syntax)
        => syntax switch
        {
            PropertySyntax syn => new PropertyModel(node, syn),
            PlaceholderSyntax syn => new PlaceholderModel(node, syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };
}
