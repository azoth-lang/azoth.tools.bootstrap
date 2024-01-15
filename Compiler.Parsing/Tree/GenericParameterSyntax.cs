using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class GenericParameterSyntax : Syntax, IGenericParameterSyntax
{
    public Variance Variance { get; }
    public SimpleName Name { get; }
    public Promise<GenericParameterTypeSymbol> Symbol { get; } = new();

    public GenericParameterSyntax(TextSpan span, Variance variance, SimpleName name)
        : base(span)
    {
        Variance = variance;
        Name = name;
    }

    public override string ToString()
        => Variance == Variance.Invariant ? Name.ToString() : $"{Variance.ToSourceCodeString()} {Name}";
}
