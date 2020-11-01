using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CST
{
    /// <summary>
    /// Things that can be declared outside of a class
    /// </summary>
    public partial interface INonMemberDeclarationSyntax
    {
        NamespaceOrPackageSymbol ContainingNamespaceSymbol { get; set; }
    }
}
