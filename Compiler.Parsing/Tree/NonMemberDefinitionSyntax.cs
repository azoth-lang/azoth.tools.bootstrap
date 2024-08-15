using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class NonMemberDefinitionSyntax : DefinitionSyntax, INonMemberDefinitionSyntax
{
    public NamespaceName ContainingNamespaceName { get; }

    protected NonMemberDefinitionSyntax(
        NamespaceName containingNamespaceName,
        TextSpan span,
        CodeFile file,
        TypeName? name,
        TextSpan nameSpan)
        : base(span, file, name, nameSpan)
    {
        ContainingNamespaceName = containingNamespaceName;
    }
}
