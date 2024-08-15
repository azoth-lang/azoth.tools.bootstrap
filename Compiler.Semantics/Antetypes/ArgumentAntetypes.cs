using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal readonly record struct ArgumentAntetypes
{
    public static ArgumentAntetypes ForFunction(IEnumerable<IMaybeExpressionAntetype> arguments)
        => new(null, arguments);

    public static ArgumentAntetypes ForConstructor(IEnumerable<IMaybeExpressionAntetype> arguments)
        => new(null, arguments);

    public static ArgumentAntetypes ForInitializer(IEnumerable<IMaybeExpressionAntetype> arguments)
        => new(null, arguments);

    public static ArgumentAntetypes ForMethod(IMaybeExpressionAntetype self, IEnumerable<IMaybeExpressionAntetype> arguments)
        => new(self, arguments);

    public int Arity => Arguments.Count;
    /// <summary>
    /// The antetype of the object that the method is called on.
    /// </summary>
    /// <remarks>This is <see langword="null"/> for functions, but also for constructors and
    /// initializers because the value will be created as part of the call and will therefore always
    /// be of the correct type.</remarks>
    public IMaybeExpressionAntetype? Self { get; init; }
    public IFixedList<IMaybeExpressionAntetype> Arguments { get; init; }

    private ArgumentAntetypes(IMaybeExpressionAntetype? self, IEnumerable<IMaybeExpressionAntetype> arguments)
    {
        Self = self;
        Arguments = arguments.ToFixedList();
    }
}
