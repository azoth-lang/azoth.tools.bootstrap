using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;

public sealed class SymbolBuilderContext(Diagnostics diagnostics)
{
    public Diagnostics Diagnostics { get; } = diagnostics;
}
