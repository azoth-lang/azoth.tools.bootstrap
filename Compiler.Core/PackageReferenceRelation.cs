namespace Azoth.Tools.Bootstrap.Compiler.Core;

public enum PackageReferenceRelation
{
    /// <summary>
    /// A dependency that is only used for and available to tests, examples, and benchmarks.
    /// </summary>
    Dev = 1,

    /// <summary>
    /// A dependency that is used internally by the implementation of this package but is not exposed
    /// through the published API.
    /// </summary>
    /// <remarks>Internal dependencies are an implementation detail.</remarks>
    Internal,

    /// <summary>
    /// A dependency that is exposed as part of the published API. Such dependencies must version
    /// match with the consumer of the package.
    /// </summary>
    Published,
}
