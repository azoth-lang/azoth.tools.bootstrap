using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public class AnyAntetype : INonVoidAntetype, IDeclaredAntetype
{
    #region Singleton
    internal static readonly AnyAntetype Instance = new();

    private AnyAntetype()
    {
    }
    #endregion

    public SpecialTypeName Name => SpecialTypeName.Any;

    IFixedList<AntetypeGenericParameter> IDeclaredAntetype.GenericParameters
        => FixedList.Empty<AntetypeGenericParameter>();

    IFixedList<GenericParameterAntetype> IDeclaredAntetype.GenericParameterAntetypes
        => FixedList.Empty<GenericParameterAntetype>();

    public IAntetype With(IEnumerable<IAntetype> typeArguments)
    {
        if (typeArguments.Any())
            throw new ArgumentException("Any type cannot have type arguments", nameof(typeArguments));
        return this;
    }
}
