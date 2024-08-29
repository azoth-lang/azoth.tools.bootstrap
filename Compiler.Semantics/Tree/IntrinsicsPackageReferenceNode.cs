using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal class IntrinsicsPackageReferenceNode : ChildNode, IPackageReferenceNode
{
    public override IPackageReferenceSyntax? Syntax => null;

    public IdentifierName AliasOrName => PackageSymbols.PackageSymbol.Name;
    public IPackageSymbols PackageSymbols => IntrinsicPackageSymbol.Instance;
    public bool IsTrusted => true;
    public IPackageSymbolNode SymbolNode { get; }

    /// <remarks>Not a singleton, because the parent node needs attached for each tree.</remarks>
    public IntrinsicsPackageReferenceNode()
    {
        SymbolNode = Child.Attach(this, SymbolNodeAspect.PackageReference_SymbolNode(this));
    }

    private class IntrinsicPackageSymbol : IPackageSymbols
    {
        #region Singleton
        public static IntrinsicPackageSymbol Instance { get; } = new IntrinsicPackageSymbol();

        private IntrinsicPackageSymbol() { }
        #endregion

        public PackageSymbol PackageSymbol => Intrinsic.Package;
        public FixedSymbolTree SymbolTree => Intrinsic.SymbolTree;
        public FixedSymbolTree TestingSymbolTree { get; }
            = new(Intrinsic.Package, FixedDictionary<Symbol, IFixedSet<Symbol>>.Empty);
    }
}
