using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[Closed(typeof(OrdinaryTypeConstructor), typeof(UserNonGenericNominalAntetype))]
public interface IOrdinaryTypeConstructor : ITypeConstructor
{
    IdentifierName ContainingPackage { get; }
    NamespaceName ContainingNamespace { get; }
    StandardName Name { get; }
    IFixedSet<NominalAntetype> Supertypes { get; }

    // TODO this seems like the wrong way to do this and was introduced only for the legacy reference equality operator
    bool HasReferenceSemantics { get; }

    new NominalAntetype With(IEnumerable<IAntetype> typeArguments);
    IAntetype ITypeConstructor.With(IEnumerable<IAntetype> typeArguments) => With(typeArguments);

    new NominalAntetype WithGenericParameterAntetypes() => With(GenericParameterAntetypes);
    IAntetype ITypeConstructor.WithGenericParameterAntetypes() => WithGenericParameterAntetypes();
}
