using Azoth.Tools.Bootstrap.Compiler.API;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Lab.Build;
internal static class ProjectRelationExtensions
{
    public static PackageReferenceRelation? ToPackageReferenceRelation(this ProjectRelation relation)
        => relation switch
        {
            ProjectRelation.None => null,
            ProjectRelation.Dev => PackageReferenceRelation.Dev,
            ProjectRelation.Internal => PackageReferenceRelation.Internal,
            ProjectRelation.Published => PackageReferenceRelation.Published,
            _ => throw ExhaustiveMatch.Failed(relation),
        };
}
