using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class CompilationUnitNode : CodeNode, ICompilationUnitNode
{
    public override ICompilationUnitSyntax Syntax { get; }

    public override CodeFile File => Syntax.File;

    public IPackageFacetNode ContainingDeclaration
        => (IPackageFacetNode)Parent.InheritedContainingDeclaration(this, this, GrammarAttribute.CurrentInheritanceContext());
    public PackageSymbol ContainingSymbol => ContainingDeclaration.PackageSymbol;
    public NamespaceName ImplicitNamespaceName => Syntax.ImplicitNamespaceName;

    private ValueAttribute<INamespaceDefinitionNode> implicitNamespaceDeclaration;
    public INamespaceDefinitionNode ImplicitNamespace
        => implicitNamespaceDeclaration.TryGetValue(out var value) ? value
            : implicitNamespaceDeclaration.GetValue(this, SymbolNodeAspect.CompilationUnit_ImplicitNamespaceDeclaration);
    public NamespaceSymbol ImplicitNamespaceSymbol => ImplicitNamespace.Symbol;
    public IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    public IFixedList<INamespaceBlockMemberDefinitionNode> Definitions { get; }
    public NamespaceScope ContainingLexicalScope
        => (NamespaceScope)InheritedContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.CompilationUnit_LexicalScope, ReferenceEqualityComparer.Instance);
    private ValueAttribute<DiagnosticCollection> diagnostics;
    public DiagnosticCollection Diagnostics
        => diagnostics.TryGetValue(out var value) ? value : diagnostics.GetValue(GetDiagnostics);

    public CompilationUnitNode(
        ICompilationUnitSyntax syntax,
        IEnumerable<IUsingDirectiveNode> usingDirectives,
        IEnumerable<INamespaceBlockMemberDefinitionNode> declarations)
    {
        Syntax = syntax;
        UsingDirectives = ChildList.Attach(this, usingDirectives);
        Definitions = ChildList.Attach(this, declarations);
    }

    internal override ISymbolDeclarationNode InheritedContainingDeclaration(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => SymbolNodeAspect.CompilationUnit_InheritedContainingDeclaration(this);

    internal override CodeFile InheritedFile(IChildNode child, IChildNode descendant)
        => FileAspect.CompilationUnit_InheritedFile(this);

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => LexicalScope;

    private DiagnosticCollection GetDiagnostics()
    {
        var diagnostics = new DiagnosticCollectionBuilder();
        CollectDiagnostics(diagnostics);
        return diagnostics.Build();
    }

    protected override void CollectDiagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        DiagnosticsAspect.CompilationUnit_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
