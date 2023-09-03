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
    public Name Name { [DebuggerStepThrough] get; }
    public Promise<int?> DeclarationNumber { [DebuggerStepThrough] get; } = new Promise<int?>();
    public Promise<VariableSymbol> Symbol { [DebuggerStepThrough] get; } = new Promise<VariableSymbol>();
    IPromise<BindingSymbol> IBindingSyntax.Symbol => Symbol;
    IPromise<NamedBindingSymbol> ILocalBindingSyntax.Symbol => Symbol;
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public ITypeSyntax? Type { [DebuggerStepThrough] get; }
    public IReferenceCapabilitySyntax? Capability { [DebuggerStepThrough] get; }
    public IExpressionSyntax? Initializer { [DebuggerStepThrough] get; }

    public VariableDeclarationStatementSyntax(
        TextSpan span,
        bool isMutableBinding,
        Name name,
        TextSpan nameSpan,
        ITypeSyntax? typeSyntax,
        IReferenceCapabilitySyntax? capability,
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
        var initializer = Initializer != null ? " = " + Initializer : "";
        return $"{binding} {Name}{type}{initializer};";
    }
}
