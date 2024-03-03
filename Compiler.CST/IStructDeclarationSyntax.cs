using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public partial interface IStructDeclarationSyntax
{
    void CreateDefaultInitializer(ISymbolTreeBuilder symbolTree);
}
