using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class BindingPatternSyntax : Syntax, IBindingPatternSyntax
{
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public Promise<int?> DeclarationNumber { [DebuggerStepThrough] get; } = new Promise<int?>();

    public Promise<NamedVariableSymbol> Symbol { [DebuggerStepThrough] get; } = new Promise<NamedVariableSymbol>();
    TextSpan ILocalBindingSyntax.NameSpan => Span;
    IPromise<BindingSymbol> IBindingSyntax.Symbol { [DebuggerStepThrough] get => Symbol; }

    public BindingPatternSyntax(TextSpan span, bool isMutableBinding, IdentifierName name)
        : base(span)
    {
        IsMutableBinding = isMutableBinding;
        Name = name;
    }

    public override string ToString() => Name.ToString();
}
