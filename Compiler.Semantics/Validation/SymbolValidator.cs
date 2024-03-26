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
                ValidateSymbol(syn, syn.Symbol);
                Walk(syn.BaseTypeName);
                // Don't recur into body, we will see those as separate members
                return;
            case IFieldDeclarationSyntax syn:
                ValidateSymbol(syn, syn.Symbol);
                break;
            case IEntityDeclarationSyntax syn:
                ValidateSymbol(syn, syn.Symbol);
                break;
            case INamedParameterSyntax syn:
                ValidateSymbol(syn, syn.Symbol);
                break;
            case IFieldParameterSyntax syn:
                ValidateReferencedSymbol(syn, syn.ReferencedSymbol);
                break;
            case ISelfParameterSyntax syn:
                ValidateSymbol(syn, syn.Symbol);
                break;
            case IVariableDeclarationStatementSyntax syn:
                ValidateSymbol(syn, syn.Symbol);
                break;
            case IForeachExpressionSyntax syn:
                ValidateSymbol(syn, syn.Symbol);
                ValidateReferencedSymbol(syn, syn.IterateMethod, optional: true);
                ValidateReferencedSymbol(syn, syn.NextMethod);
                break;
            case IBindingPatternSyntax syn:
                ValidateSymbol(syn, syn.Symbol);
                break;
            case IDeclarationSyntax syn:
                ValidateSymbol(syn, syn.Symbol);
                break;
            case ISimpleNameExpressionSyntax syn:
                ValidateReferencedSymbol(syn, syn.ReferencedSymbol, optional: true);
                break;
            case ISelfExpressionSyntax syn:
                ValidateReferencedSymbol(syn, syn.ReferencedSymbol);
                break;
            case ITypeNameSyntax syn:
                ValidateReferencedSymbol(syn, syn.ReferencedSymbol);
                break;
            // TODO this is complex because the symbol hasn't been made yet when the viewpoint type is resolved
            //case ISelfViewpointTypeSyntax syn:
            //    ValidateReferencedSymbol(syn, syn.ReferencedSymbol);
            //    break;
            case IMoveExpressionSyntax syn:
                ValidateReferencedSymbol(syn, syn.ReferencedSymbol);
                break;
            case INewObjectExpressionSyntax syn:
                ValidateReferencedSymbol(syn, syn.ReferencedSymbol);
                break;
            case IInvocationExpressionSyntax syn:
                ValidateReferencedSymbol(syn, syn.ReferencedSymbol, optional: true);
                break;
            case IMemberAccessExpressionSyntax syn:
                ValidateReferencedSymbol(syn, syn.ReferencedSymbol, optional: true);
                break;
            case INameExpressionSyntax syn:
                ValidateReferencedSymbol(syn, syn.ReferencedSymbol);
                break;
        }

        WalkChildren(syntax);
    }

    private void ValidateSymbol(ISyntax syntax, IPromise<Symbol?> promise)
    {
        if (!promise.IsFulfilled)
            throw new Exception($"Syntax doesn't have a symbol '{syntax}'");

        if (promise.Result is null)
            throw new Exception($"Syntax has unknown symbol '{syntax}'");

        if (!symbolTree.Contains(promise.Result))
            throw new Exception($"Symbol isn't in the symbol tree '{promise.Result}'");
    }

    private static void ValidateReferencedSymbol(ISyntax syntax, IPromise<Symbol?> promise, bool optional = false)
    {
        if (!promise.IsFulfilled)
            throw new Exception($"Syntax doesn't have a referenced symbol '{syntax}'");

        if (!optional && promise.Result is null)
            throw new Exception($"Syntax has unknown referenced symbol '{syntax}'");
    }
}
