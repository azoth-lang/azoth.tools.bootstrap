using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public partial interface IClassDeclarationSyntax
{
    void CreateDefaultConstructor(SymbolTreeBuilder symbolTree);
}
