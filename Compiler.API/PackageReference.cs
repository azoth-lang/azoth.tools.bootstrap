using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.API;

public sealed class PackageReference
{
    /// <summary>
    /// The name of the referenced package.
    /// </summary>
    public IdentifierName Name { get; }

    /// <summary>
    /// If an alias was assigned to this reference, what the alias is.
    /// </summary>
    public IdentifierName? Alias { get; }

    /// <summary>
    /// The name of this package as seen by the code.
    /// </summary>
    /// <remarks>If this reference assigns an <see cref="Alias"/>, the <see cref="EffectiveName"/>
    /// will be the alias, otherwise it will be <see cref="Name"/>.</remarks>
    public IdentifierName EffectiveName => Alias ?? Name;

    /// <summary>
    /// Whether the referenced package is trusted by the current package.
    /// </summary>
    public bool IsTrusted { get; }

    // TODO add Package ID here which could be either a hash or path (hash is used when there is a lock file)

    public PackageReferenceRelation Relation { get; }

    /// <summary>
    /// Whether the test facet should reference the test facet of the referenced package
    /// </summary>
    public bool ReferenceTests { get; }

    public PackageReference(IdentifierName name, IdentifierName? alias, bool isTrusted, PackageReferenceRelation relation, bool referenceTests)
    {
        Name = name;
        Alias = alias;
        IsTrusted = isTrusted;
        Relation = relation;
        ReferenceTests = referenceTests;
    }

    internal IPackageReferenceSyntax ToSyntax()
        => IPackageReferenceSyntax.Create(EffectiveName, Alias, Name, IsTrusted, Relation, ReferenceTests);
}
