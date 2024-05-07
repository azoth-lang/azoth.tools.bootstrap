using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
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
    public Promise<IDeclaredUserType> ContainingDeclaredType => InheritedContainingDeclaredType();
    private ValueAttribute<GenericParameterType> type;
    public GenericParameterType Type
        => type.TryGetValue(out var value) ? value : type.GetValue(this, TypeAttributes.GenericParameter);

    private ValueAttribute<GenericParameterTypeSymbol> symbol;
    public GenericParameterTypeSymbol Symbol
        => symbol.TryGetValue(out var value) ? value : symbol.GetValue(this, SymbolAttribute.GenericParameter);

    public GenericParameterNode(IGenericParameterSyntax syntax, ICapabilityConstraintNode constraint)
    {
        Syntax = syntax;
        Constraint = constraint;
    }
}
