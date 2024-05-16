using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

/// <summary>
/// Things that can be declared outside of a class.
/// </summary>
public partial interface INonMemberDefinitionSyntax
{
    NamespaceSymbol ContainingNamespaceSymbol { get; set; }
}
