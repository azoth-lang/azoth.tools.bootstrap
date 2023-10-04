using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic;

public readonly struct ArgumentResults
{
    public ExpressionResult? Self { get; }
    public FixedList<ExpressionResult> Arguments { get; }
    public IEnumerable<ExpressionResult> All => Self.YieldValue().Concat(Arguments);

    public ArgumentResults(ExpressionResult? self, IEnumerable<ExpressionResult> arguments)
    {
        Self = self;
        Arguments = arguments.ToFixedList();
    }
}
