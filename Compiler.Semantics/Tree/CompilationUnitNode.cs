using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
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
        => (IPackageFacetNode)Parent.InheritedContainingDeclaration(this, this);
    public PackageSymbol ContainingSymbol => ContainingDeclaration.Symbol;
    public NamespaceName ImplicitNamespaceName => Syntax.ImplicitNamespaceName;

    private ValueAttribute<INamespaceDefinitionNode> implicitNamespaceDeclaration;
    public INamespaceDefinitionNode ImplicitNamespace
        => implicitNamespaceDeclaration.TryGetValue(out var value) ? value
            : implicitNamespaceDeclaration.GetValue(this, SymbolNodeAttributes.CompilationUnit_ImplicitNamespaceDeclaration);
    public NamespaceSymbol ImplicitNamespaceSymbol => ImplicitNamespace.Symbol;
    public IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    public IFixedList<INamespaceBlockMemberDefinitionNode> Definitions { get; }
    public NamespaceScope ContainingLexicalScope => (NamespaceScope)Parent.InheritedContainingLexicalScope(this, this);
    private ValueAttribute<LexicalScope> lexicalScope;
    public LexicalScope LexicalScope
        => lexicalScope.TryGetValue(out var value) ? value
            : lexicalScope.GetValue(this, LexicalScopeAttributes.CompilationUnit);

    private ValueAttribute<IFixedList<Diagnostic>> diagnostics;
    public IFixedList<Diagnostic> Diagnostics
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

    internal override INamespaceDeclarationNode InheritedContainingDeclaration(IChildNode caller, IChildNode child)
        => SymbolNodeAttributes.CompilationUnit_InheritedContainingDeclaration(this);

    internal override CodeFile InheritedFile(IChildNode caller, IChildNode child)
        => FileAttribute.CompilationUnit_InheritedFile(this);

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode caller, IChildNode child)
        => LexicalScope;

    private IFixedList<Diagnostic> GetDiagnostics()
    {
        var diagnostics = new Diagnostics();
        CollectDiagnostics(diagnostics);
        return diagnostics.Build();
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        DiagnosticsAttribute.CompilationUnit_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
