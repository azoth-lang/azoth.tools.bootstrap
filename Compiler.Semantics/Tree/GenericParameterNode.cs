using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class GenericParameterNode : CodeNode, IGenericParameterNode
{
    public override IGenericParameterSyntax Syntax { get; }
    public ICapabilityConstraintNode Constraint { get; }
    public IdentifierName Name => Syntax.Name;
    private ValueAttribute<IPackageFacetDeclarationNode> facet;
    public IPackageFacetDeclarationNode Facet
        => facet.TryGetValue(out var value) ? value
            : facet.GetValue(InheritedFacet);
    public ParameterIndependence Independence => Syntax.Independence;
    public ParameterVariance Variance => Syntax.Variance;
    private ValueAttribute<GenericParameter> parameter;
    public GenericParameter Parameter
        => parameter.TryGetValue(out var value) ? value
            : parameter.GetValue(this, TypeDeclarationsAspect.GenericParameter_Parameter);

    private ValueAttribute<IDeclaredUserType> containingDeclaredType;
    public IDeclaredUserType ContainingDeclaredType
        => containingDeclaredType.TryGetValue(out var value) ? value
            : containingDeclaredType.GetValue(InheritedContainingDeclaredType);

    private ValueAttribute<GenericParameterType> declaredType;
    public GenericParameterType DeclaredType
        => declaredType.TryGetValue(out var value) ? value
            : declaredType.GetValue(this, TypeDeclarationsAspect.GenericParameter_DeclaredType);

    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)InheritedContainingDeclaration();
    public UserTypeSymbol ContainingSymbol => ContainingDeclaration.Symbol;
    private ValueAttribute<GenericParameterTypeSymbol> symbol;
    public GenericParameterTypeSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAspect.GenericParameter);
    public IFixedSet<ITypeMemberDefinitionNode> Members => FixedSet.Empty<IAlwaysTypeMemberDefinitionNode>();
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;

    public GenericParameterNode(IGenericParameterSyntax syntax, ICapabilityConstraintNode constraint)
    {
        Syntax = syntax;
        Constraint = constraint;
    }

    public IEnumerable<IInstanceMemberDeclarationNode> InstanceMembersNamed(StandardName named)
        // TODO should look up members based on generic constraints
        => Enumerable.Empty<IInstanceMemberDeclarationNode>();

    public IEnumerable<IAssociatedMemberDeclarationNode> AssociatedMembersNamed(StandardName named)
        // TODO should look up members based on generic constraints
        => Enumerable.Empty<IAssociatedMemberDeclarationNode>();
}
