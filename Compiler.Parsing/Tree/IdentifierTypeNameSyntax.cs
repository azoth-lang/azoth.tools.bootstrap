using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

/// <summary>
/// The unqualified name of a non-special type.
/// </summary>
internal sealed class IdentifierTypeNameSyntax : TypeSyntax, IIdentifierTypeNameSyntax
{
    public IdentifierName Name { get; }
    public Promise<TypeSymbol?> ReferencedSymbol { get; } = new Promise<TypeSymbol?>();
    public BareReferenceType? NamedBareType => (NamedType as ReferenceType)?.BareType;

    public IdentifierTypeNameSyntax(TextSpan span, string name)
        : base(span)
    {
        Name = name;
    }

    public override string ToString() => Name.ToString();
}
