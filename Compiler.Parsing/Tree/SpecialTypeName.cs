using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

/// <summary>
/// The unqualified name of a type.
/// </summary>
internal sealed class SpecialTypeNameSyntax : TypeSyntax, ISpecialTypeNameSyntax
{
    public SpecialTypeName Name { get; }
    public Promise<TypeSymbol?> ReferencedSymbol { get; } = new Promise<TypeSymbol?>();

    public SpecialTypeNameSyntax(TextSpan span, SpecialTypeName name)
        : base(span)
    {
        Name = name;
    }

    public override string ToString() => Name.ToString();
}
