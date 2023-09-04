using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal abstract class Declaration : AbstractSyntax, IDeclaration
{
    public CodeFile File { get; }
    public Symbol Symbol { get; }
    public TextSpan NameSpan { get; }

    protected Declaration(CodeFile file, TextSpan span, Symbol symbol, TextSpan nameSpan)
        : base(span)
    {
        Symbol = symbol;
        NameSpan = nameSpan;
        File = file;
    }
}
