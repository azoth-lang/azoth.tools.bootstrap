using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

internal static partial class BindingNamesAspect
{
    #region Invocation Expressions
    public static partial IGetterMethodDeclarationNode? GetterInvocationExpression_ReferencedDeclaration(IGetterInvocationExpressionNode node)
        => node.ReferencedPropertyAccessors.OfType<IGetterMethodDeclarationNode>().TrySingle();

    public static partial ISetterMethodDeclarationNode? SetterInvocationExpression_ReferencedDeclaration(ISetterInvocationExpressionNode node)
        => node.ReferencedPropertyAccessors.OfType<ISetterMethodDeclarationNode>().TrySingle();
    #endregion

    #region Name Expressions
    public static partial ISelfParameterNode? SelfExpression_ReferencedDefinition(ISelfExpressionNode node)
        => node.ContainingDeclaration switch
        {
            IMethodDefinitionNode n => n.SelfParameter,
            ISourceConstructorDefinitionNode n => n.SelfParameter,
            IDefaultConstructorDefinitionNode _
                => throw new UnreachableException("A `self` expression cannot occur here because it has an empty body."),
            ISourceInitializerDefinitionNode n => n.SelfParameter,
            IDefaultInitializerDefinitionNode _
                => throw new UnreachableException("A `self` expression cannot occur here because it has an empty body."),
            IFieldDefinitionNode _ => null,
            IAssociatedFunctionDefinitionNode _ => null,
            IFunctionDefinitionNode _ => null,
            _ => throw ExhaustiveMatch.Failed(node.ContainingDeclaration),
        };

    public static partial void SelfExpression_Contribute_Diagnostics(ISelfExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.ContainingDeclaration is not (IMethodDefinitionNode or ISourceConstructorDefinitionNode or IInitializerDeclarationNode))
            diagnostics.Add(node.IsImplicit
                ? OtherSemanticError.ImplicitSelfOutsideMethod(node.File, node.Syntax.Span)
                : OtherSemanticError.SelfOutsideMethod(node.File, node.Syntax.Span));
    }
    #endregion

    public static partial IFixedSet<IConstructorDeclarationNode> NewObjectExpression_ReferencedConstructors(INewObjectExpressionNode node)
    {
        var typeDeclarationNode = node.PackageNameScope().Lookup(node.ConstructingAntetype);
        if (typeDeclarationNode is null)
            return FixedSet.Empty<IConstructorDeclarationNode>();

        if (node.ConstructorName is null)
            return typeDeclarationNode.Members.OfType<IConstructorDeclarationNode>()
                                      .Where(c => c.Name is null).ToFixedSet();

        return typeDeclarationNode.AssociatedMembersNamed(node.ConstructorName)
                                  .OfType<IConstructorDeclarationNode>().ToFixedSet();
    }

    public static partial void Attribute_Contribute_Diagnostics(IAttributeNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.ReferencedSymbol is null)
            diagnostics.Add(NameBindingError.CouldNotBindName(node.File, node.TypeName.Syntax.Span));
    }
}
