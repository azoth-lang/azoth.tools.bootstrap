using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class GenericParameterSyntax : Syntax, IGenericParameterSyntax
{
    public Name Name { get; }

    public GenericParameterSyntax(TextSpan span, Name name)
        : base(span)
    {
        Name = name;
    }

    public override string ToString() => Name.ToString();
}
