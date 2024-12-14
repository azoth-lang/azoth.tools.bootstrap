using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

internal static partial class BindingNamesAspect
{
    #region Attributes
    public static partial void Attribute_Contribute_Diagnostics(IAttributeNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.ReferencedSymbol is null)
            diagnostics.Add(NameBindingError.CouldNotBindName(node.File, node.TypeName.Syntax.Span));
    }
    #endregion

    #region Types
    public static partial ITypeDeclarationNode? SpecialTypeName_ReferencedDeclaration(ISpecialTypeNameNode node)
        // TODO report error for use of `Self` outside of a type
        => node.ContainingLexicalScope.Lookup(node.Name);
    #endregion

    #region Expressions
    public static partial IFixedSet<IConstructorDeclarationNode> NewObjectExpression_ReferencedConstructors(INewObjectExpressionNode node)
    {
        var typeDeclarationNode = node.PackageNameScope().Lookup(node.ConstructingPlainType);
        if (typeDeclarationNode is null)
            return FixedSet.Empty<IConstructorDeclarationNode>();

        if (node.ConstructorName is null)
            return typeDeclarationNode.Members.OfType<IConstructorDeclarationNode>()
                                      .Where(c => c.Name is null).ToFixedSet();

        return typeDeclarationNode.AssociatedMembersNamed(node.ConstructorName)
                                  .OfType<IConstructorDeclarationNode>().ToFixedSet();
    }
    #endregion

    #region Invocation Expressions
    public static partial IGetterMethodDeclarationNode? GetterInvocationExpression_ReferencedDeclaration(IGetterInvocationExpressionNode node)
        => node.ReferencedPropertyAccessors.OfType<IGetterMethodDeclarationNode>().TrySingle();

    public static partial ISetterMethodDeclarationNode? SetterInvocationExpression_ReferencedDeclaration(ISetterInvocationExpressionNode node)
        => node.ReferencedPropertyAccessors.OfType<ISetterMethodDeclarationNode>().TrySingle();
    #endregion

    #region Name Expressions
    public static partial ITypeDeclarationNode? SpecialTypeNameExpression_ReferencedDeclaration(ISpecialTypeNameExpressionNode node)
        // TODO report error for use of `Self` outside of a type
        => node.ContainingLexicalScope.Lookup(node.Name);

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
}
