using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

public readonly record struct ArgumentPlainTypes
{
    public static ArgumentPlainTypes ForFunction(IEnumerable<IMaybePlainType> arguments)
        => new(null, arguments);

    public static ArgumentPlainTypes ForConstructor(IEnumerable<IMaybePlainType> arguments)
        => new(null, arguments);

    public static ArgumentPlainTypes ForInitializer(IEnumerable<IMaybePlainType> arguments)
        => new(null, arguments);

    public static ArgumentPlainTypes ForMethod(IMaybePlainType self, IEnumerable<IMaybePlainType> arguments)
        => new(self, arguments);

    public int Arity => Arguments.Count;
    /// <summary>
    /// The plainType of the object that the method is called on.
    /// </summary>
    /// <remarks>This is <see langword="null"/> for functions, but also for constructors and
    /// initializers because the value will be created as part of the call and will therefore always
    /// be of the correct type.</remarks>
    public IMaybePlainType? Self { get; init; }
    public IFixedList<IMaybePlainType> Arguments { get; init; }

    private ArgumentPlainTypes(IMaybePlainType? self, IEnumerable<IMaybePlainType> arguments)
    {
        Self = self;
        Arguments = arguments.ToFixedList();
    }
}
