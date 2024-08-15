namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

/// <summary>
/// Syntax that corresponds to Azoth code.
/// </summary>
public partial interface ICodeSyntax : ISyntax
{
    /// <summary>
    /// A developer/debugging friendly string representation of the syntax.
    /// </summary>
    string ToString();
}
