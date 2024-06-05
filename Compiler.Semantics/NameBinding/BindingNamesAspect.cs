using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

internal static class BindingNamesAspect
{
    public static ISelfParameterNode? SelfExpression_ReferencedParameter(ISelfExpressionNode node)
        => node.ContainingDeclaration switch
        {
            IConcreteMethodDefinitionNode n => n.SelfParameter,
            ISourceConstructorDefinitionNode n => n.SelfParameter,
            IDefaultConstructorDefinitionNode _
                => throw new UnreachableException("A `self` expression cannot occur here because it has an empty body."),
            IInitializerDefinitionNode n => n.SelfParameter,
            IFieldDefinitionNode _ => null,
            IAssociatedFunctionDefinitionNode _ => null,
            IFunctionDefinitionNode _ => null,
            _ => throw ExhaustiveMatch.Failed(node.ContainingDeclaration),
        };

    public static void SelfExpression_ContributeDiagnostics(ISelfExpressionNode node, Diagnostics diagnostics)
    {
        if (node.ContainingDeclaration is not (IMethodDefinitionNode or ISourceConstructorDefinitionNode or IInitializerDeclarationNode))
            diagnostics.Add(node.IsImplicit
                ? OtherSemanticError.ImplicitSelfOutsideMethod(node.File, node.Syntax.Span)
                : OtherSemanticError.SelfOutsideMethod(node.File, node.Syntax.Span));
    }

    public static IFixedSet<IConstructorDeclarationNode> NewObjectExpression_ReferencedConstructors(INewObjectExpressionNode node)
    {
        var typeDeclarationNode = node.InheritedPackageNameScope().Lookup(node.ConstructingAntetype);
        if (typeDeclarationNode is null)
            return FixedSet.Empty<IConstructorDeclarationNode>();

        if (node.ConstructorName is null)
            return typeDeclarationNode.Members.OfType<IConstructorDeclarationNode>()
                                      .Where(c => c.Name is null).ToFixedSet();

        return typeDeclarationNode.AssociatedMembersNamed(node.ConstructorName)
                                  .OfType<IConstructorDeclarationNode>().ToFixedSet();
    }
}
