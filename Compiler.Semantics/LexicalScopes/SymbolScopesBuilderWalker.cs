using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Walkers;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

internal class SymbolScopesBuilderWalker : SyntaxWalker<SymbolScope>
{
    private readonly NestedSymbolScope globalScope;
    private readonly FixedDictionary<NamespaceName, NamespaceSymbolScope> namespaces;

    public SymbolScopesBuilderWalker(
        NestedSymbolScope globalScope,
        FixedDictionary<NamespaceName, NamespaceSymbolScope> namespaces)
    {
        this.globalScope = globalScope;
        this.namespaces = namespaces;
    }

    public void BuildFor(ICompilationUnitSyntax compilationUnit, SymbolScope containingScope)
        => Walk(compilationUnit, containingScope);

    protected override void WalkNonNull(IConcreteSyntax syntax, SymbolScope containingScope)
    {
        // Forward calls for expressions before setting ContainingLexicalScope
        if (syntax is IExpressionSyntax exp)
        {
            _ = Walk(exp, containingScope);
            return;
        }

        if (syntax is IHasContainingLexicalScope hasContainingLexicalScope)
            hasContainingLexicalScope.ContainingLexicalScope = containingScope;

        switch (syntax)
        {
            case ICompilationUnitSyntax syn:
                containingScope = BuildNamespaceScopes(NamespaceName.Global, syn.ImplicitNamespaceName, containingScope);
                containingScope = BuildUsingDirectivesScope(syn.UsingDirectives, containingScope);
                break;
            case INamespaceDefinitionSyntax syn:
                if (syn.IsGlobalQualified)
                {
                    containingScope = globalScope;
                    containingScope = BuildNamespaceScopes(NamespaceName.Global, syn.FullName, containingScope);
                }
                else
                    containingScope = BuildNamespaceScopes(syn.ContainingNamespaceName, syn.DeclaredNames, containingScope);

                containingScope = BuildUsingDirectivesScope(syn.UsingDirectives, containingScope);
                break;
            case ITypeDefinitionSyntax syn:
                foreach (var genericParameter in syn.GenericParameters)
                    WalkNonNull(genericParameter, containingScope);
                containingScope = BuildTypeParameterScope(syn, containingScope);
                if (syn is IClassDefinitionSyntax classSyntax)
                    Walk(classSyntax.BaseTypeName, containingScope);
                foreach (var supertypeName in syn.SupertypeNames)
                    WalkNonNull(supertypeName, containingScope);
                containingScope = BuildTypeBodyScope(syn, containingScope);
                foreach (var member in syn.Members)
                    WalkNonNull(member, containingScope);
                return;
            case IFunctionDefinitionSyntax function:
                foreach (var attribute in function.Attributes)
                    WalkNonNull(attribute, containingScope);
                foreach (var parameter in function.Parameters)
                    WalkNonNull(parameter, containingScope);
                Walk(function.Return, containingScope);
                containingScope = BuildBodyScope(function.Parameters, containingScope);
                WalkNonNull(function.Body, containingScope);
                return;
            case IAssociatedFunctionDefinitionSyntax function:
                foreach (var parameter in function.Parameters)
                    Walk(parameter, containingScope);
                Walk(function.Return, containingScope);
                containingScope = BuildBodyScope(function.Parameters, containingScope);
                WalkNonNull(function.Body, containingScope);
                return;
            case IConcreteMethodDefinitionSyntax concreteMethod:
                WalkNonNull(concreteMethod.SelfParameter, containingScope);
                foreach (var parameter in concreteMethod.Parameters)
                    Walk(parameter, containingScope);
                Walk(concreteMethod.Return, containingScope);
                containingScope = BuildBodyScope(concreteMethod.Parameters, containingScope);
                WalkNonNull(concreteMethod.Body, containingScope);
                return;
            case IConstructorDefinitionSyntax constructor:
                Walk(constructor.SelfParameter, containingScope);
                foreach (var parameter in constructor.Parameters)
                    Walk(parameter, containingScope);
                containingScope = BuildBodyScope(constructor.Parameters, containingScope);
                WalkNonNull(constructor.Body, containingScope);
                return;
            case IBodyOrBlockSyntax bodyOrBlock:
                WalkNonNull(bodyOrBlock, containingScope);
                return;
            case IExpressionSyntax _:
                throw new UnreachableException($"{nameof(IExpressionSyntax)} should be unreachable `{syntax}`");
        }

        WalkChildren(syntax, containingScope);
    }

    private void WalkNonNull(IBodyOrBlockSyntax bodyOrBlock, SymbolScope containingScope)
    {
        foreach (var statement in bodyOrBlock.Statements)
        {
            Walk(statement, containingScope);
            // Each variable declaration effectively starts a new scope after it, this
            // ensures a lookup returns the last declaration
            if (statement is IVariableDeclarationStatementSyntax variableDeclaration)
                containingScope = BuildVariableScope(containingScope, variableDeclaration.Name, variableDeclaration.Symbol);
        }
    }

    private ConditionalSymbolScopes Walk(IExpressionSyntax? syntax, SymbolScope containingScope)
    {
        if (syntax is IHasContainingLexicalScope hasContainingLexicalScope)
            hasContainingLexicalScope.ContainingLexicalScope = containingScope;

        switch (syntax)
        {
            default:
                throw ExhaustiveMatch.Failed(syntax);
            case IMemberAccessExpressionSyntax exp:
                return Walk(exp.Context, containingScope);
            case IAssignmentExpressionSyntax exp:
                _ = Walk(exp.LeftOperand, containingScope);
                return Walk(exp.RightOperand, containingScope);
            case IBinaryOperatorExpressionSyntax exp:
                switch (exp.Operator)
                {
                    case BinaryOperator.Or:
                        var flowScope = Walk(exp.LeftOperand, containingScope).False;
                        _ = Walk(exp.RightOperand, flowScope);
                        break;
                    case BinaryOperator.And:
                    default:
                        containingScope = Walk(exp.LeftOperand, containingScope).True;
                        return Walk(exp.RightOperand, containingScope);
                }
                break;
            case IBlockExpressionSyntax block:
                WalkNonNull(block, containingScope);
                break;
            case IBreakExpressionSyntax exp:
                _ = Walk(exp.Value, containingScope);
                break;
            case IConversionExpressionSyntax exp:
            {
                var scopes = Walk(exp.Referent, containingScope);
                Walk(exp.ConvertToType, containingScope);
                return scopes;
            }
            case IForeachExpressionSyntax exp:
            {
                Walk(exp.Type, containingScope);
                var bodyScope = Walk(exp.InExpression, containingScope).True;
                bodyScope = BuildVariableScope(bodyScope, exp.VariableName, exp.Symbol);
                WalkNonNull(exp.Block, bodyScope);
                break;
            }
            case IFreezeExpressionSyntax exp:
                return Walk(exp.Referent, containingScope);
            case IIdExpressionSyntax exp:
                return Walk(exp.Referent, containingScope);
            case IInvocationExpressionSyntax exp:
                containingScope = Walk(exp.Expression, containingScope).True;
                foreach (var argument in exp.Arguments)
                    containingScope = Walk(argument, containingScope).True;
                break;
            case ILoopExpressionSyntax exp:
                WalkNonNull(exp.Block, containingScope);
                break;
            case IMoveExpressionSyntax exp:
                return Walk(exp.Referent, containingScope);
            case INewObjectExpressionSyntax exp:
                WalkNonNull(exp.Type, containingScope);
                foreach (var argument in exp.Arguments)
                    containingScope = Walk(argument, containingScope).True;
                break;
            case IPatternMatchExpressionSyntax exp:
                containingScope = Walk(exp.Referent, containingScope).True;
                return WalkNonNull(exp.Pattern, containingScope);
            case IReturnExpressionSyntax exp:
                _ = Walk(exp.Value, containingScope);
                break;
            case IUnaryOperatorExpressionSyntax exp:
            {
                var scopes = Walk(exp.Operand, containingScope);
                if (exp.Operator == UnaryOperator.Not) scopes = scopes.Swapped();
                return scopes;
            }
            case IUnsafeExpressionSyntax exp:
                return Walk(exp.Expression, containingScope);
            case IWhileExpressionSyntax exp:
            {
                var bodyScope = Walk(exp.Condition, containingScope).True;
                WalkNonNull(exp.Block, bodyScope);
                break;
            }
            case IIfExpressionSyntax exp:
            {
                var conditionScopes = Walk(exp.Condition, containingScope);
                Walk(exp.ThenBlock, conditionScopes.True);
                if (exp.ElseClause is not null)
                    Walk(exp.ElseClause, conditionScopes.False);
                break;
            }
            case IAsyncBlockExpressionSyntax exp:
            {
                // TODO once `async let` is supported, create a lexical scope for the variable
                WalkNonNull(exp.Block, containingScope);
                break;
            }
            case IAsyncStartExpressionSyntax exp:
                Walk(exp.Expression, containingScope);
                break;
            case IAwaitExpressionSyntax exp:
                return Walk(exp.Expression, containingScope);
            case IGenericNameExpressionSyntax exp:
                foreach (var typeArgument in exp.TypeArguments)
                    WalkNonNull(typeArgument, containingScope);
                break;
            case ILiteralExpressionSyntax _:
            case ISelfExpressionSyntax _:
            case IIdentifierNameExpressionSyntax _:
            case ISpecialTypeNameExpressionSyntax _:
            case INextExpressionSyntax _:
            case null:
                // Nothing special to do
                break;
        }

        return ConditionalSymbolScopes.Unconditional(containingScope);
    }

    private ConditionalSymbolScopes WalkNonNull(IPatternSyntax syntax, SymbolScope containingScope)
    {
        switch (syntax)
        {
            default:
                throw ExhaustiveMatch.Failed(syntax);
            case IBindingContextPatternSyntax pat:
            {
                var scopes = WalkNonNull(pat.Pattern, containingScope);
                Walk(pat.Type, containingScope);
                return scopes;
            }
            case IBindingPatternSyntax pat:
                var trueScope = BuildVariableScope(containingScope, pat.Name, pat.Symbol);
                return new ConditionalSymbolScopes(trueScope, containingScope);
            case IOptionalPatternSyntax pat:
                return WalkNonNull(pat.Pattern, containingScope);
        }
    }

    private SymbolScope BuildNamespaceScopes(
        NamespaceName containingNamespaceName,
        NamespaceName declaredNamespaceNames,
        SymbolScope containingScope)
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

    private NestedSymbolScope BuildNamespaceScope(NamespaceName nsName, SymbolScope containingScope)
    {
        var ns = namespaces[nsName];
        return NestedSymbolScope.Create(containingScope, ns.SymbolsInPackage, ns.NestedSymbolsInPackage);
    }

    private SymbolScope BuildUsingDirectivesScope(
        IFixedList<IUsingDirectiveSyntax> usingDirectives,
        SymbolScope containingScope)
    {
        if (!usingDirectives.Any()) return containingScope;

        var importedSymbols = new Dictionary<TypeName, HashSet<IPromise<Symbol>>>();
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
        return NestedSymbolScope.Create(containingScope, symbolsInScope);
    }

    private static NestedSymbolScope BuildTypeParameterScope(
        ITypeDefinitionSyntax typeSyntax,
        SymbolScope containingScope)
    {
        var symbols = typeSyntax.GenericParameters.ToFixedDictionary(p => (TypeName)p.Name,
                p => FixedSet.Create<IPromise<Symbol>>(p.Symbol));

        return NestedSymbolScope.Create(containingScope, symbols);
    }

    private static NestedSymbolScope BuildTypeBodyScope(ITypeDefinitionSyntax typeSyntax, SymbolScope containingScope)
    {
        // Only "static" names are in scope. Other names must use `self.`
        var symbols = typeSyntax.Members.OfType<IAssociatedFunctionDefinitionSyntax>()
                                .GroupBy(m => m.Name, m => m.Symbol).ToDictionary(e => (TypeName)e.Key,
                                    e => e.ToFixedSet<IPromise<Symbol>>());

        return NestedSymbolScope.Create(containingScope, symbols.ToFixedDictionary());
    }

    private static NestedSymbolScope BuildBodyScope(
        IEnumerable<IConstructorOrInitializerParameterSyntax> parameters,
        SymbolScope containingScope)
    {
        var symbols = parameters.OfType<INamedParameterSyntax>()
                                .GroupBy(p => p.Name, p => p.Symbol)
                                .ToFixedDictionary(e => (TypeName)e.Key, e => e.ToFixedSet<IPromise<Symbol>>());
        return NestedSymbolScope.Create(containingScope, symbols);
    }

    private static NestedSymbolScope BuildVariableScope(
        SymbolScope containingScope,
        IdentifierName name,
        IPromise<NamedVariableSymbol> symbol)
    {
        var symbols = new Dictionary<TypeName, IFixedSet<IPromise<Symbol>>>()
        {
            { name, symbol.Yield().ToFixedSet<IPromise<Symbol>>() }
        }.ToFixedDictionary();
        return NestedSymbolScope.Create(containingScope, symbols);
    }
}
