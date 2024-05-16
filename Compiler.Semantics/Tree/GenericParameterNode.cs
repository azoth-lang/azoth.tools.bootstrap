using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class GenericParameterNode : CodeNode, IGenericParameterNode
{
    public override IGenericParameterSyntax Syntax { get; }
    public ICapabilityConstraintNode Constraint { get; }
    public IdentifierName Name => Syntax.Name;
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

    public IUserTypeDeclarationNode ContainingDeclarationNode
        => (IUserTypeDeclarationNode)InheritedContainingDeclaration();
    public UserTypeSymbol ContainingSymbol => ContainingDeclarationNode.Symbol;
    private ValueAttribute<GenericParameterTypeSymbol> symbol;
    public GenericParameterTypeSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAttribute.GenericParameter);

    public GenericParameterNode(IGenericParameterSyntax syntax, ICapabilityConstraintNode constraint)
    {
        Syntax = syntax;
        Constraint = constraint;
    }
}
