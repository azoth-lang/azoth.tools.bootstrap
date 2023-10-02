using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic;

public readonly struct ArgumentResults
{
    public ExpressionResult? SelfArgument { get; }
    public FixedList<ExpressionResult> Expressions { get; }

    public ArgumentResults(ExpressionResult? selfArgument, IEnumerable<ExpressionResult> arguments)
    {
        SelfArgument = selfArgument;
        Expressions = arguments.ToFixedList();
    }

    public IEnumerable<DataType> ToArgumentTypes() => Expressions.Select(a => a.Type);
}
