using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Semantics;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class MissingNameSyntax : NameExpressionSyntax, IMissingNameSyntax
{
    public override IPromise<DataType> DataType => Types.DataType.PromiseOfUnknown;
    public override Promise<UnknownNameSyntax> Semantics { get; }
        = Promise.ForValue(UnknownNameSyntax.Instance);

    public MissingNameSyntax(TextSpan span)
        : base(span) { }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;
    public override string ToString() => "⧼unknown⧽";
}
