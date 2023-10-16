using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class GenericParameterSyntax : Syntax, IGenericParameterSyntax
{
    public SimpleName Name { get; }
    public Promise<GenericParameterTypeSymbol> Symbol { get; } = new();

    public GenericParameterSyntax(TextSpan span, SimpleName name)
        : base(span)
    {
        Name = name;
    }

    public override string ToString() => Name.ToString();
}
