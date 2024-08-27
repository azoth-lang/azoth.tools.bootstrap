using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
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
    private ValueAttribute<IPackageFacetDeclarationNode> facet;
    public IPackageFacetDeclarationNode Facet
        => facet.TryGetValue(out var value) ? value
            : facet.GetValue(InheritedFacet);
    public TypeParameterIndependence Independence => Syntax.Independence;
    public TypeParameterVariance Variance => Syntax.Variance;
    private ValueAttribute<GenericParameter> parameter;
    public GenericParameter Parameter
        => parameter.TryGetValue(out var value) ? value
            : parameter.GetValue(this, TypeDeclarationsAspect.GenericParameter_Parameter);
    private IDeclaredUserType? containingDeclaredType;
    private bool containingDeclaredTypeCached;
    public IDeclaredUserType ContainingDeclaredType
        => GrammarAttribute.IsCached(in containingDeclaredTypeCached) ? containingDeclaredType!
            : this.Inherited(ref containingDeclaredTypeCached, ref containingDeclaredType,
                InheritedContainingDeclaredType);
    private ValueAttribute<GenericParameterType> declaredType;
    public GenericParameterType DeclaredType
        => declaredType.TryGetValue(out var value) ? value
            : declaredType.GetValue(this, TypeDeclarationsAspect.GenericParameter_DeclaredType);
    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)InheritedContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public UserTypeSymbol ContainingSymbol => ContainingDeclaration.Symbol;
    private GenericParameterTypeSymbol? symbol;
    private bool symbolCached;
    public GenericParameterTypeSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol, SymbolsAspect.GenericParameter_Symbol);
    public IFixedSet<ITypeMemberDefinitionNode> Members => [];
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    public IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers => [];

    public GenericParameterNode(IGenericParameterSyntax syntax, ICapabilityConstraintNode constraint)
    {
        Syntax = syntax;
        Constraint = Child.Attach(this, constraint);
    }
}
