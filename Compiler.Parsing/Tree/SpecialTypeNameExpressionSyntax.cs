using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Semantics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

/// <summary>
/// A name of a variable or namespace
/// </summary>
internal sealed class SpecialTypeNameExpressionSyntax : NameExpressionSyntax, ISpecialTypeNameExpressionSyntax
{
    // A null name means this syntax was generated as an assumed missing name and the name is unknown
    public SpecialTypeName Name { get; }
    public override Promise<SpecialTypeNameExpressionSyntaxSemantics> Semantics { [DebuggerStepThrough] get; } = new();
    public override IPromise<DataType> DataType => Types.DataType.PromiseOfUnknown;
    public override Promise<TypeSymbol?> ReferencedSymbol { get; } = new Promise<TypeSymbol?>();

    public SpecialTypeNameExpressionSyntax(TextSpan span, SpecialTypeName name)
        : base(span)
    {
        Name = name;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;
    public override string ToString() => Name.ToString();
}
