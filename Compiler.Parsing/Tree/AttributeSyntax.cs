using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class AttributeSyntax : CodeSyntax, IAttributeSyntax
{
    public IStandardTypeNameSyntax TypeName { get; }

    public AttributeSyntax(TextSpan span, IStandardTypeNameSyntax typeName)
        : base(span)
    {
        TypeName = typeName;
    }

    public override string ToString() => $"#{TypeName}";
}
