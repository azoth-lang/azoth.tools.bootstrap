using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

[Closed(typeof(UserNonGenericNominalAntetype), typeof(UserGenericNominalAntetype))]
public interface IUserNominalAntetype : INonVoidAntetype
{
    IFixedSet<NominalAntetype> Supertypes { get; }

    NominalAntetype AsNominalAntetype { get; }
}
