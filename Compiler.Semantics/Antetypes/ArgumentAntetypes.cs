using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal readonly record struct ArgumentAntetypes(IMaybeExpressionAntetype? Self, IFixedList<IMaybeExpressionAntetype> Arguments)
{
    public int Arity => Arguments.Count;

    public ArgumentAntetypes(IMaybeExpressionAntetype? self, IEnumerable<IMaybeExpressionAntetype> arguments)
        : this(self, arguments.ToFixedList())
    {
    }
}
