using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

/// <summary>
/// The unqualified name of a non-special type.
/// </summary>
internal sealed class IdentifierTypeNameSyntax : TypeSyntax, IIdentifierTypeNameSyntax
{
    public IdentifierName Name { get; }

    public IdentifierTypeNameSyntax(TextSpan span, string name)
        : base(span)
    {
        Name = name;
    }

    public override string ToString() => Name.ToString();
}
