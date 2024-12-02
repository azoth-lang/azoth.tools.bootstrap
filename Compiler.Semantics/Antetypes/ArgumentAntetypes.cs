using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

public readonly record struct ArgumentAntetypes
{
    public static ArgumentAntetypes ForFunction(IEnumerable<IMaybeAntetype> arguments)
        => new(null, arguments);

    public static ArgumentAntetypes ForConstructor(IEnumerable<IMaybeAntetype> arguments)
        => new(null, arguments);

    public static ArgumentAntetypes ForInitializer(IEnumerable<IMaybeAntetype> arguments)
        => new(null, arguments);

    public static ArgumentAntetypes ForMethod(IMaybeAntetype self, IEnumerable<IMaybeAntetype> arguments)
        => new(self, arguments);

    public int Arity => Arguments.Count;
    /// <summary>
    /// The plainType of the object that the method is called on.
    /// </summary>
    /// <remarks>This is <see langword="null"/> for functions, but also for constructors and
    /// initializers because the value will be created as part of the call and will therefore always
    /// be of the correct type.</remarks>
    public IMaybeAntetype? Self { get; init; }
    public IFixedList<IMaybeAntetype> Arguments { get; init; }

    private ArgumentAntetypes(IMaybeAntetype? self, IEnumerable<IMaybeAntetype> arguments)
    {
        Self = self;
        Arguments = arguments.ToFixedList();
    }
}
