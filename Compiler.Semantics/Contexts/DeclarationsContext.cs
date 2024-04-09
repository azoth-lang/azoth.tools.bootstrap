using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;

public sealed class DeclarationsContext
{
    public Diagnostics Diagnostics { get; init; }
    public required DeclarationTree Declarations { get; init; }

    public DeclarationsContext(SymbolBuilderContext context)
    {
        Diagnostics = context.Diagnostics;
    }

    [SetsRequiredMembers]
    public DeclarationsContext(Diagnostics diagnostics, DeclarationTree declarations)
    {
        Diagnostics = diagnostics;
        Declarations = declarations;
    }
}

// TODO fill out placeholder
public class DeclarationTree
{ }
