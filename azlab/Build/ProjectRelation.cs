namespace Azoth.Tools.Bootstrap.Lab.Build;

internal enum ProjectRelation
{
    /// <summary>
    /// A package that is not used by this package.
    /// </summary>
    /// <remarks>This exists so that packages can be bundled.</remarks>
    None,

    /// <summary>
    /// A dependency that is only used for and available to tests, examples, and benchmarks.
    /// </summary>
    Dev,

    /// <summary>
    /// A dependency that is used by the build process, including code analysis.
    /// </summary>
    Build,

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
