using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class NamedParameterSyntax : ParameterSyntax, INamedParameterSyntax
{
    public bool IsMutableBinding { get; }
    public new Name Name { get; }
    public Promise<int?> DeclarationNumber { get; } = new Promise<int?>();
    public Promise<VariableSymbol> Symbol { get; } = new Promise<VariableSymbol>();
    IPromise<BindingSymbol> IBindingSyntax.Symbol => Symbol;
    IPromise<NamedBindingSymbol> ILocalBindingSyntax.Symbol => Symbol;
    public ITypeSyntax Type { get; }
    public override IPromise<DataType> DataType { get; }
    public IExpressionSyntax? DefaultValue { get; }

    public NamedParameterSyntax(
        TextSpan span,
        bool isMutableBinding,
        Name name,
        ITypeSyntax typeSyntax,
        IExpressionSyntax? defaultValue)
        : base(span, name)
    {
        IsMutableBinding = isMutableBinding;
        Name = name;
        Type = typeSyntax;
        DefaultValue = defaultValue;
        DataType = Symbol.Select(s => s.DataType);
    }

    public override string ToString()
    {
        var defaultValue = DefaultValue != null ? " = " + DefaultValue : "";
        return $"{Name}: {Type}{defaultValue}";
    }
}
