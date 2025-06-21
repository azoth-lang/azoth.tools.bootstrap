using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Types;

public static class DeclaredCapabilityExtensions
{
    public static string ToSourceCodeString(this DeclaredCapability capability)
        => capability switch
        {
            DeclaredCapability.Constant => "const",
            DeclaredCapability.Default => "",
            DeclaredCapability.Identity => "id",
            DeclaredCapability.Isolated => "iso",
            DeclaredCapability.Mutable => "mut",
            DeclaredCapability.Read => "read",
            DeclaredCapability.TemporarilyConstant => "temp const",
            DeclaredCapability.TemporarilyIsolated => "temp iso",
            _ => throw ExhaustiveMatch.Failed(capability),
        };
}
