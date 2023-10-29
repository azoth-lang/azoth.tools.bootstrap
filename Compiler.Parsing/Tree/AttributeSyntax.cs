using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class AttributeSyntax : Syntax, IAttributeSyntax
{
    public ITypeNameSyntax TypeName { get; }

    public AttributeSyntax(TextSpan span, ITypeNameSyntax typeName)
        : base(span)
    {
        TypeName = typeName;
    }

    public override string ToString() => $"#{TypeName}";
}
