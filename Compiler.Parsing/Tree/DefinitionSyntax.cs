using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class DefinitionSyntax : Syntax, IDefinitionSyntax
{
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
