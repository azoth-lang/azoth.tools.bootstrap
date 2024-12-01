using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[Closed(typeof(OrdinaryTypeConstructor))]
public interface IOrdinaryTypeConstructor : ITypeConstructor
{
    IdentifierName ContainingPackage { get; }
    NamespaceName ContainingNamespace { get; }
    StandardName Name { get; }
    IFixedSet<NominalAntetype> Supertypes { get; }

    new NominalAntetype Construct(IEnumerable<IAntetype> typeArguments);
    IAntetype ITypeConstructor.Construct(IEnumerable<IAntetype> typeArguments) => Construct(typeArguments);

    new NominalAntetype ConstructWithGenericParameterPlayTypes() => Construct(GenericParameterPlainTypes);
    IAntetype ITypeConstructor.ConstructWithGenericParameterPlainTypes() => ConstructWithGenericParameterPlayTypes();
}
