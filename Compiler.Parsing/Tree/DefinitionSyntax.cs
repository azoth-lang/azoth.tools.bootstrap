using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class DefinitionSyntax : Syntax, IDefinitionSyntax
{
    private SymbolScope? containingLexicalScope;
    public SymbolScope ContainingLexicalScope
    {
        [DebuggerStepThrough]
        get => containingLexicalScope
               ?? throw new InvalidOperationException($"{nameof(ContainingLexicalScope)} not yet assigned");
        [DebuggerStepThrough]
        set
        {
            if (containingLexicalScope is not null)
                throw new InvalidOperationException($"Can't set {nameof(ContainingLexicalScope)} repeatedly");
            containingLexicalScope = value;
        }
    }

    public CodeFile File { get; }
    public TypeName? Name { get; }
    public TextSpan NameSpan { get; }
    public IPromise<Symbol> Symbol { get; }

    protected DefinitionSyntax(
        TextSpan span,
        CodeFile file,
        TypeName? name,
        TextSpan nameSpan,
        IPromise<Symbol> symbol)
        : base(span)
    {
        NameSpan = nameSpan;
        Symbol = symbol;
        File = file;
        Name = name;
    }
}
