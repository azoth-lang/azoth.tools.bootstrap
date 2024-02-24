using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class GenericParameterSyntax : Syntax, IGenericParameterSyntax
{
    public ParameterVariance ParameterVariance { get; }
    public SimpleName Name { get; }
    public Promise<GenericParameterTypeSymbol> Symbol { get; } = new();

    public GenericParameterSyntax(TextSpan span, SimpleName name, ParameterVariance variance)
        : base(span)
    {
        ParameterVariance = variance;
        Name = name;
    }

    public override string ToString()
        => ParameterVariance == ParameterVariance.Invariant ? Name.ToString() : $"{ParameterVariance.ToSourceCodeString()} {Name}";
}
