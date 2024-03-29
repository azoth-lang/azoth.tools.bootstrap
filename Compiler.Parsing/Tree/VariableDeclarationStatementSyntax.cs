using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class VariableDeclarationStatementSyntax : StatementSyntax, IVariableDeclarationStatementSyntax
{
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public Promise<int?> DeclarationNumber { [DebuggerStepThrough] get; } = new Promise<int?>();
    public Promise<NamedVariableSymbol> Symbol { [DebuggerStepThrough] get; } = new Promise<NamedVariableSymbol>();
    IPromise<BindingSymbol> IBindingSyntax.Symbol => Symbol;
    IPromise<NamedBindingSymbol> ILocalBindingSyntax.Symbol => Symbol;
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public ITypeSyntax? Type { [DebuggerStepThrough] get; }
    public ICapabilitySyntax? Capability { [DebuggerStepThrough] get; }
    public IExpressionSyntax? Initializer { [DebuggerStepThrough] get; }

    public VariableDeclarationStatementSyntax(
        TextSpan span,
        bool isMutableBinding,
        IdentifierName name,
        TextSpan nameSpan,
        ITypeSyntax? typeSyntax,
        ICapabilitySyntax? capability,
        IExpressionSyntax? initializer)
        : base(span)
    {
        IsMutableBinding = isMutableBinding;
        Name = name;
        NameSpan = nameSpan;
        Type = typeSyntax;
        Capability = capability;
        Initializer = initializer;
    }

    public override string ToString()
    {
        var binding = IsMutableBinding ? "var" : "let";
        var type = Type is not null ? ": " + Type : "";
        if (Capability is not null) type = ": " + Capability;
        var initializer = Initializer is not null ? " = " + Initializer : "";
        return $"{binding} {Name}{type}{initializer};";
    }
}
