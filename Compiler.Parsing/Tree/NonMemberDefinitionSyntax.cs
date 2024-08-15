using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class NonMemberDefinitionSyntax : DefinitionSyntax, INonMemberDefinitionSyntax
{
    protected NonMemberDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        TypeName? name,
        TextSpan nameSpan)
        : base(span, file, name, nameSpan)
    {
    }
}
