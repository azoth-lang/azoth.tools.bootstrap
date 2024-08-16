using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

/// <summary>
/// A name of a variable or namespace
/// </summary>
internal sealed class SpecialTypeNameExpressionSyntax : NameExpressionSyntax, ISpecialTypeNameExpressionSyntax
{
    public SpecialTypeName Name { get; }

    public SpecialTypeNameExpressionSyntax(TextSpan span, SpecialTypeName name)
        : base(span)
    {
        Name = name;
    }

    public override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;
    public override string ToString() => Name.ToString();
}
