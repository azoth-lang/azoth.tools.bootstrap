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
    public bool IsLentBinding { get; }
    public new SimpleName Name { get; }
    public Promise<int?> DeclarationNumber { get; } = new Promise<int?>();
    public Promise<NamedVariableSymbol> Symbol { get; } = new Promise<NamedVariableSymbol>();
    IPromise<BindingSymbol> IBindingSyntax.Symbol => Symbol;
    IPromise<NamedBindingSymbol> ILocalBindingSyntax.Symbol => Symbol;
    public ITypeSyntax Type { get; }
    public override IPromise<DataType> DataType { get; }
    public IExpressionSyntax? DefaultValue { get; }

    public NamedParameterSyntax(
        TextSpan span,
        bool isMutableBinding,
        bool isLentBinding,
        SimpleName name,
        ITypeSyntax typeSyntax,
        IExpressionSyntax? defaultValue)
        : base(span, name)
    {
        IsMutableBinding = isMutableBinding;
        Name = name;
        Type = typeSyntax;
        DefaultValue = defaultValue;
        IsLentBinding = isLentBinding;
        DataType = Symbol.Select(s => s.Type);
    }

    public override string ToString()
    {
        var lent = IsLentBinding ? "lent " : "";
        var mutable = IsMutableBinding ? "var " : "";
        var defaultValue = DefaultValue is not null ? " = " + DefaultValue : "";
        return $"{lent}{mutable}{Name}: {Type}{defaultValue}";
    }
}
