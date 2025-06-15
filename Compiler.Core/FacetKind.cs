namespace Azoth.Tools.Bootstrap.Compiler.Core;

/// <summary>
/// The kind of package facet.
/// </summary>
public enum FacetKind
{
    /// <summary>
    /// The main facet is the standard code for the package.
    /// </summary>
    Main = 1,

    /// <summary>
    /// The tests facet contains all the test code for the package.
    /// </summary>
    Tests,

    // TODO `Examples,` compiles the example code in doc comments
}
