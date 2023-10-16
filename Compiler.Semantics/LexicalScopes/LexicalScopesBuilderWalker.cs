using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Walkers;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

internal class LexicalScopesBuilderWalker : SyntaxWalker<LexicalScope>
{
    private readonly NestedScope globalScope;
    private readonly FixedDictionary<NamespaceName, Namespace> namespaces;

    public LexicalScopesBuilderWalker(
        NestedScope globalScope,
        FixedDictionary<NamespaceName, Namespace> namespaces)
    {
        this.globalScope = globalScope;
        this.namespaces = namespaces;
    }

    public void BuildFor(ICompilationUnitSyntax compilationUnit, LexicalScope containingScope)
        => Walk(compilationUnit, containingScope);

    protected override void WalkNonNull(ISyntax syntax, LexicalScope containingScope)
    {
        if (syntax is IHasContainingLexicalScope hasContainingLexicalScope)
            hasContainingLexicalScope.ContainingLexicalScope = containingScope;

        switch (syntax)
        {
            case ICompilationUnitSyntax syn:
                containingScope = BuildNamespaceScopes(NamespaceName.Global, syn.ImplicitNamespaceName, containingScope);
                containingScope = BuildUsingDirectivesScope(syn.UsingDirectives, containingScope);
                break;
            case INamespaceDeclarationSyntax syn:
                if (syn.IsGlobalQualified)
                {
                    containingScope = globalScope;
                    containingScope = BuildNamespaceScopes(NamespaceName.Global, syn.FullName, containingScope);
                }
                else
                    containingScope = BuildNamespaceScopes(syn.ContainingNamespaceName, syn.DeclaredNames, containingScope);

                containingScope = BuildUsingDirectivesScope(syn.UsingDirectives, containingScope);
                break;
            case ITypeDeclarationSyntax syn:
                foreach (var genericParameter in syn.GenericParameters)
                    Walk(genericParameter, containingScope);
                containingScope = BuildTypeParameterScope(syn, containingScope);
                if (syn is IClassDeclarationSyntax classSyntax)
                    Walk(classSyntax.BaseTypeName, containingScope);
                foreach (var supertypeName in syn.SupertypeNames)
                    Walk(supertypeName, containingScope);
                containingScope = BuildTypeBodyScope(syn, containingScope);
                foreach (var member in syn.Members)
                    Walk(member, containingScope);
                return;
            case IFunctionDeclarationSyntax function:
                foreach (var parameter in function.Parameters)
                    Walk(parameter, containingScope);
                Walk(function.Return, containingScope);
                containingScope = BuildBodyScope(function.Parameters, containingScope);
                Walk(function.Body, containingScope);
                return;
            case IAssociatedFunctionDeclarationSyntax function:
                foreach (var parameter in function.Parameters)
                    Walk(parameter, containingScope);
                Walk(function.Return, containingScope);
                containingScope = BuildBodyScope(function.Parameters, containingScope);
                Walk(function.Body, containingScope);
                return;
            case IConcreteMethodDeclarationSyntax concreteMethod:
                Walk(concreteMethod.SelfParameter, containingScope);
                foreach (var parameter in concreteMethod.Parameters)
                    Walk(parameter, containingScope);
                Walk(concreteMethod.Return, containingScope);
                containingScope = BuildBodyScope(concreteMethod.Parameters, containingScope);
                Walk(concreteMethod.Body, containingScope);
                return;
            case IConstructorDeclarationSyntax constructor:
                Walk(constructor.SelfParameter, containingScope);
                foreach (var parameter in constructor.Parameters)
                    Walk(parameter, containingScope);
                containingScope = BuildBodyScope(constructor.Parameters, containingScope);
                Walk(constructor.Body, containingScope);
                return;
            case IBodyOrBlockSyntax bodyOrBlock:
                foreach (var statement in bodyOrBlock.Statements)
                {
                    Walk(statement, containingScope);
                    // Each variable declaration effectively starts a new scope after it, this
                    // ensures a lookup returns the last declaration
                    if (statement is IVariableDeclarationStatementSyntax variableDeclaration)
                        containingScope = BuildVariableScope(containingScope, variableDeclaration.Name, variableDeclaration.Symbol);
                }
                return;
            case IForeachExpressionSyntax foreachExpression:
                Walk(foreachExpression.Type, containingScope);
                Walk(foreachExpression.InExpression, containingScope);
                containingScope = BuildVariableScope(containingScope, foreachExpression.VariableName, foreachExpression.Symbol);
                Walk(foreachExpression.Block, containingScope);
                return;
        }

        WalkChildren(syntax, containingScope);
    }

    private LexicalScope BuildNamespaceScopes(
        NamespaceName containingNamespaceName,
        NamespaceName declaredNamespaceNames,
        LexicalScope containingScope)
    {
        foreach (var name in declaredNamespaceNames.NamespaceNames())
        {
            var fullNamespaceName = containingNamespaceName.Qualify(name);
            // Skip the global namespace because we already have the global lexical scopes
            if (fullNamespaceName == NamespaceName.Global) continue;
            containingScope = BuildNamespaceScope(fullNamespaceName, containingScope);
        }

        return containingScope;
    }

    private LexicalScope BuildNamespaceScope(NamespaceName nsName, LexicalScope containingScope)
    {
        var ns = namespaces[nsName];
        return NestedScope.Create(containingScope, ns.SymbolsInPackage, ns.NestedSymbolsInPackage);
    }

    private LexicalScope BuildUsingDirectivesScope(
        FixedList<IUsingDirectiveSyntax> usingDirectives,
        LexicalScope containingScope)
    {
        if (!usingDirectives.Any()) return containingScope;

        var importedSymbols = new Dictionary<Name, HashSet<IPromise<Symbol>>>();
        foreach (var usingDirective in usingDirectives)
        {
            if (!namespaces.TryGetValue(usingDirective.Name, out var ns))
            {
                // TODO diagnostics.Add(NameBindingError.UsingNonExistentNamespace(file, usingDirective.Span, usingDirective.Name));
                continue;
            }

            foreach (var (name, additionalSymbols) in ns.Symbols)
            {
                if (importedSymbols.TryGetValue(name, out var symbols))
                    symbols.AddRange(additionalSymbols);
                else
                    importedSymbols.Add(name, additionalSymbols.ToHashSet());
            }
        }

        var symbolsInScope = importedSymbols.ToFixedDictionary(e => e.Key, e => e.Value.ToFixedSet());
        return NestedScope.Create(containingScope, symbolsInScope);
    }

    private static LexicalScope BuildTypeParameterScope(
        ITypeDeclarationSyntax typeSyntax,
        LexicalScope containingScope)
    {
        var symbols = typeSyntax.GenericParameters.ToFixedDictionary(p => (Name)p.Name,
                p => FixedSet.Create<IPromise<Symbol>>(p.Symbol));

        return NestedScope.Create(containingScope, symbols);
    }

    private static LexicalScope BuildTypeBodyScope(ITypeDeclarationSyntax typeSyntax, LexicalScope containingScope)
    {
        // Only "static" names are in scope. Other names must use `self.`
        var symbols = typeSyntax.Members.OfType<IAssociatedFunctionDeclarationSyntax>()
                                .GroupBy(m => m.Name, m => m.Symbol).ToDictionary(e => (Name)e.Key,
                                    e => e.ToFixedSet<IPromise<Symbol>>());

        return NestedScope.Create(containingScope, symbols.ToFixedDictionary());
    }

    private static LexicalScope BuildBodyScope(
        IEnumerable<IConstructorParameterSyntax> parameters,
        LexicalScope containingScope)
    {
        var symbols = parameters.OfType<INamedParameterSyntax>()
                                .GroupBy(p => p.Name, p => p.Symbol)
                                .ToFixedDictionary(e => (Name)e.Key, e => e.ToFixedSet<IPromise<Symbol>>());
        return NestedScope.Create(containingScope, symbols);
    }

    private static LexicalScope BuildVariableScope(
        LexicalScope containingScope,
        SimpleName name,
        IPromise<VariableSymbol> symbol)
    {
        var symbols = new Dictionary<Name, FixedSet<IPromise<Symbol>>>()
        {
            { name, symbol.Yield().ToFixedSet<IPromise<Symbol>>() }
        }.ToFixedDictionary();
        return NestedScope.Create(containingScope, symbols);
    }
}
