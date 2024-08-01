using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public interface IPackageSyntax : ISyntax
{
    IdentifierName Name { get; }
    ISymbolTreeBuilder SymbolTree { get; }
    ISymbolTreeBuilder TestingSymbolTree { get; }
    SymbolForest SymbolTrees { get; }
    SymbolForest TestingSymbolTrees { get; }
    IFixedSet<ICompilationUnitSyntax> CompilationUnits { get; }
    IFixedSet<ICompilationUnitSyntax> TestingCompilationUnits { get; }
    IFixedSet<IEntityDefinitionSyntax> EntityDeclarations { get; }
    IFixedSet<IEntityDefinitionSyntax> TestingEntityDeclarations { get; }
    IFixedSet<IPackageReferenceSyntax> References { get; }

    /// <summary>
    /// All the entity declarations including both regular code and testing code.
    /// </summary>
    IFixedSet<IEntityDefinitionSyntax> AllEntityDeclarations { get; }

    Diagnostics Diagnostics { get; }
}
