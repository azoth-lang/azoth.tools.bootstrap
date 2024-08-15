using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class FunctionTypeSyntax : TypeSyntax, IFunctionTypeSyntax
{
    public IFixedList<IParameterTypeSyntax> Parameters { get; }
    public IReturnTypeSyntax Return { get; }

    public FunctionTypeSyntax(TextSpan span, IFixedList<IParameterTypeSyntax> parameters, IReturnTypeSyntax @return)
        : base(span)
    {
        Parameters = parameters;
        Return = @return;
    }

    public override string ToString()
        => $"({string.Join(", ", Parameters.Select(p => p.ToString()))}) -> {Return}";
}
