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
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class GenericParameterNode : CodeNode, IGenericParameterNode
{
    public override IGenericParameterSyntax Syntax { get; }
    public ICapabilityConstraintNode Constraint { get; }
    public IdentifierName Name => Syntax.Name;
    public IFixedSet<BareReferenceType> Supertypes => FixedSet.Empty<BareReferenceType>();
    public bool SupertypesFormCycle => false;
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

    private IDeclaredUserType? containingDeclaredType;
    private bool containingDeclaredTypeCached;
    public IDeclaredUserType ContainingDeclaredType
        => GrammarAttribute.IsCached(in containingDeclaredTypeCached) ? containingDeclaredType!
            : GrammarAttribute.Inherited(ref containingDeclaredTypeCached, this,
                InheritedContainingDeclaredType, ref containingDeclaredType);

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
    public IFixedSet<ITypeMemberDefinitionNode> Members
        => FixedSet.Empty<ITypeMemberDefinitionNode>();
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    public IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers
        => FixedSet.Empty<ITypeMemberDefinitionNode>();

    public GenericParameterNode(IGenericParameterSyntax syntax, ICapabilityConstraintNode constraint)
    {
        Syntax = syntax;
        Constraint = constraint;
    }

    public IEnumerable<IInstanceMemberDeclarationNode> InclusiveInstanceMembersNamed(StandardName named)
        // TODO should look up members based on generic constraints
        => Enumerable.Empty<IInstanceMemberDeclarationNode>();

    public IEnumerable<IAssociatedMemberDeclarationNode> AssociatedMembersNamed(StandardName named)
        // TODO should look up members based on generic constraints
        => Enumerable.Empty<IAssociatedMemberDeclarationNode>();
}
