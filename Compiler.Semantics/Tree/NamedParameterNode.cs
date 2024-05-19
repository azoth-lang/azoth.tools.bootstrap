using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class NamedParameterNode : ParameterNode, INamedParameterNode
{
    public override INamedParameterSyntax Syntax { get; }
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public bool IsLentBinding => Syntax.IsLentBinding;
    public override IdentifierName Name => Syntax.Name;
    public int? DeclarationNumber => Syntax.DeclarationNumber.Result;
    public ITypeNode TypeNode { get; }
    private ValueAttribute<DataType> type;
    public override DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeMemberDeclarationsAspect.NamedParameterNode_Type);
    private ValueAttribute<Parameter> parameterType;
    public Parameter ParameterType
        => parameterType.TryGetValue(out var value) ? value
            : parameterType.GetValue(this, TypeMemberDeclarationsAspect.NamedParameter_ParameterType);
    private ValueAttribute<NamedVariableSymbol> symbol;
    public NamedVariableSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAttribute.NamedParameter_Symbol);

    public NamedParameterNode(INamedParameterSyntax syntax, ITypeNode type)
    {
        Syntax = syntax;
        TypeNode = Child.Attach(this, type);
    }
}
