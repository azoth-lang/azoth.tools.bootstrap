using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class DefinitionSyntax : CodeSyntax, IDefinitionSyntax
{
    public CodeFile File { get; }
    public TypeName? Name { get; }
    public TextSpan NameSpan { get; }

    protected DefinitionSyntax(
        TextSpan span,
        CodeFile file,
        TypeName? name,
        TextSpan nameSpan)
        : base(span)
    {
        NameSpan = nameSpan;
        File = file;
        Name = name;
    }
}
