using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Walkers;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;

public class SymbolValidator : SyntaxWalker
{
    private readonly ISymbolTree symbolTree;

    public SymbolValidator(ISymbolTree symbolTree)
    {
        this.symbolTree = symbolTree;
    }

    /// <summary>
    /// Validate that the entities have symbols and those symbols are in the symbol tree.
    /// </summary>
    public void Validate(IEnumerable<IEntityDeclarationSyntax> entityDeclaration)
    {
        foreach (var declaration in entityDeclaration)
            WalkNonNull(declaration);
    }

    protected override void WalkNonNull(ISyntax syntax)
    {
        switch (syntax)
        {
            case IClassDeclarationSyntax syn:
                CheckSymbol(syn, syn.Symbol);
                Walk(syn.BaseTypeName);
                // Don't recur into body, we will see those as separate members
                return;
            case IFieldDeclarationSyntax syn:
                CheckSymbol(syn, syn.Symbol);
                break;
            case IEntityDeclarationSyntax syn:
                CheckSymbol(syn, syn.Symbol);
                break;
            case INamedParameterSyntax syn:
                CheckSymbol(syn, syn.Symbol);
                break;
            case IFieldParameterSyntax syn:
                CheckReferencedSymbol(syn, syn.ReferencedSymbol);
                break;
            case ISelfParameterSyntax syn:
                CheckSymbol(syn, syn.Symbol);
                break;
            case IVariableDeclarationStatementSyntax syn:
                CheckSymbol(syn, syn.Symbol);
                break;
            case IBindingPatternSyntax syn:
                CheckSymbol(syn, syn.Symbol);
                break;
            case IDeclarationSyntax syn:
                CheckSymbol(syn, syn.Symbol);
                break;
            case ISimpleNameExpressionSyntax syn:
                CheckReferencedSymbol(syn, syn.ReferencedSymbol);
                break;
            case ISelfExpressionSyntax syn:
                CheckReferencedSymbol(syn, syn.ReferencedSymbol);
                break;
            case ITypeNameSyntax syn:
                CheckReferencedSymbol(syn, syn.ReferencedSymbol);
                break;
            case IMoveExpressionSyntax syn:
                CheckReferencedSymbol(syn, syn.ReferencedSymbol);
                break;
            case INewObjectExpressionSyntax syn:
                CheckReferencedSymbol(syn, syn.ReferencedSymbol);
                break;
            case IInvocationExpressionSyntax syn:
                CheckReferencedSymbol(syn, syn.ReferencedSymbol);
                break;
        }

        WalkChildren(syntax);
    }

    private void CheckSymbol(ISyntax syntax, IPromise<Symbol?> promise)
    {
        if (!promise.IsFulfilled)
            throw new Exception($"Syntax doesn't have a symbol '{syntax}'");

        if (promise.Result is null)
            throw new Exception($"Syntax has unknown symbol '{syntax}'");

        if (!symbolTree.Contains(promise.Result))
            throw new Exception($"Symbol isn't in the symbol tree '{promise.Result}'");
    }

    private static void CheckReferencedSymbol(ISyntax syntax, IPromise<Symbol?> promise)
    {
        if (!promise.IsFulfilled)
            throw new Exception($"Syntax doesn't have a referenced symbol '{syntax}'");

        if (promise.Result is null)
            throw new Exception($"Syntax has unknown referenced symbol '{syntax}'");
    }
}
