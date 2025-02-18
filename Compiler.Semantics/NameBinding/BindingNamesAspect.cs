using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

internal static partial class BindingNamesAspect
{
    #region Attributes
    public static partial void Attribute_Contribute_Diagnostics(IAttributeNode node, DiagnosticCollectionBuilder diagnostics)
    {
        // TODO don't do this on symbols. Do proper initializer resolution
        if (node.ReferencedSymbol is null)
            diagnostics.Add(NameBindingError.CouldNotBindName(node.File, node.TypeName.Syntax.Span));
    }
    #endregion

    #region Parameters
    public static partial IFieldDefinitionNode? FieldParameter_ReferencedField(IFieldParameterNode node)
        // TODO report error for field parameter without referenced field
        => node.ContainingTypeDefinition.Members.OfType<IFieldDefinitionNode>()
               .FirstOrDefault(f => f.Name == node.Name);
    #endregion

    #region Name Expressions
    public static partial ISelfParameterNode? SelfExpression_ReferencedDefinition(ISelfExpressionNode node)
        // TODO use a more specific inherited attribute? (e.g. SelfParameter)
        // TODO or add `SelfParameter` to ExecutableDefinition and do node.ContainingDeclaration.SelfParameter?
        => node.ContainingDeclaration switch
        {
            IMethodDefinitionNode n => n.SelfParameter,
            IOrdinaryInitializerDefinitionNode n => n.SelfParameter,
            IDefaultInitializerDefinitionNode _
                => throw new UnreachableException("A `self` expression cannot occur here because it has an empty body."),
            IFieldDefinitionNode _ => null,
            IAssociatedFunctionDefinitionNode _ => null,
            IFunctionDefinitionNode _ => null,
            _ => throw ExhaustiveMatch.Failed(node.ContainingDeclaration),
        };

    public static partial void SelfExpression_Contribute_Diagnostics(ISelfExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.ContainingDeclaration is not (IMethodDefinitionNode or IInitializerDeclarationNode))
            diagnostics.Add(node.IsImplicit
                ? OtherSemanticError.ImplicitSelfOutsideMethod(node.File, node.Syntax.Span)
                : OtherSemanticError.SelfOutsideMethod(node.File, node.Syntax.Span));
    }
    #endregion

    #region Type Names
    public static partial ITypeDeclarationNode? BuiltInTypeName_ReferencedDeclaration(IBuiltInTypeNameNode node)
        // TODO report error for use of `Self` outside of a type
        => node.ContainingLexicalScope.Lookup(node.Name);

    public static partial ITypeDeclarationNode? OrdinaryTypeName_ReferencedDeclaration(IOrdinaryTypeNameNode node)
    {
        // TODO this prefers the non-suffixed name. Maybe it should be the other way around to avoid conflict
        var typeDeclaration = node.ContainingLexicalScope.LookupTypeDeclarations(node.Name).TrySingle();
        if (node.IsAttributeType)
            typeDeclaration ??= node.ContainingLexicalScope.LookupTypeDeclarations(node.Name, withAttributeSuffix: true).TrySingle();
        return typeDeclaration;
    }

    public static partial void OrdinaryTypeName_Contribute_Diagnostics(
        IOrdinaryTypeNameNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (node.ReferencedDeclaration is not null) return;
        var declarations = node.ContainingLexicalScope.LookupTypeDeclarations(node.Name);
        switch (declarations.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindName(node.File, node.Syntax.Span));
                break;
            case 1:
                // If there is only one match, then ReferencedSymbol is not null
                throw new UnreachableException();
            default:
                diagnostics.Add(NameBindingError.AmbiguousName(node.File, node.Syntax.Span));
                break;
        }
    }

    public static partial ITypeDeclarationNode? QualifiedTypeName_ReferencedDeclaration(IQualifiedTypeNameNode node)
        // TODO do proper generic type selection
        => node.Context.ReferencedDeclaration?.TypeMembersNamed(node.Name).TrySingle();

    private static IFixedSet<ITypeDeclarationNode> LookupTypeDeclarations(
        this LexicalScope containingLexicalScope,
        OrdinaryName name,
        bool withAttributeSuffix = false)
    {
        if (withAttributeSuffix) name = name.WithAttributeSuffix();
        return containingLexicalScope.Lookup<ITypeDeclarationNode>(name).ToFixedSet();
    }
    #endregion
}
