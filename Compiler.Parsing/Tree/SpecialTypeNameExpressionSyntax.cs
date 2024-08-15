using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

/// <summary>
/// A name of a variable or namespace
/// </summary>
internal sealed class SpecialTypeNameExpressionSyntax : NameExpressionSyntax, ISpecialTypeNameExpressionSyntax
{
    public SpecialTypeName Name { get; }
    public override IPromise<DataType> DataType => Types.DataType.PromiseOfUnknown;

    public SpecialTypeNameExpressionSyntax(TextSpan span, SpecialTypeName name)
        : base(span)
    {
        Name = name;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;
    public override string ToString() => Name.ToString();
}
