using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic;

public readonly record struct ArgumentResults(ExpressionResult? Self, FixedList<ExpressionResult> Arguments)
{
    public IEnumerable<ExpressionResult> All => Self.YieldValue().Concat(Arguments);

    public ArgumentResults(ExpressionResult? self, IEnumerable<ExpressionResult> arguments)
        : this(self, arguments.ToFixedList())
    {
    }
}
