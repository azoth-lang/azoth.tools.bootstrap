using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic;

/// <summary>
/// The basic analyzer does name binding, type checking and constant folding
/// within statements and expressions. Evaluating parameter and return types
/// has already been completed as part of symbol binding. This class handles
/// declarations and delegates expressions, types etc. to other classes.
///
/// All basic analysis uses specific terminology to distinguish different
/// aspects of type checking. (The entry method `Check` is an exception. It
/// is named to match other analyzers but performs a resolve.)
///
/// Terminology:
///
/// * Resolve: includes type inference and checking
/// * Check: check something has an expected type
/// * Infer: infer what type (or sometimes symbol) something has
/// * Evaluate: determine the type for some type syntax
/// </summary>
public class BasicAnalyzer
{
    private readonly SymbolTreeBuilder symbolTreeBuilder;
    private readonly SymbolForest symbolTrees;
    private readonly ObjectTypeSymbol? stringSymbol;
    private readonly Diagnostics diagnostics;

    private BasicAnalyzer(
        SymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        ObjectTypeSymbol? stringSymbol,
        Diagnostics diagnostics)
    {
        this.symbolTreeBuilder = symbolTreeBuilder;
        this.symbolTrees = symbolTrees;
        this.stringSymbol = stringSymbol;
        this.diagnostics = diagnostics;
    }

    public static void Check(PackageSyntax<Package> package, ObjectTypeSymbol? stringSymbol)
    {
        var analyzer = new BasicAnalyzer(package.SymbolTree, package.SymbolTrees, stringSymbol, package.Diagnostics);
        analyzer.Resolve(package.EntityDeclarations);
    }

    private void Resolve(FixedSet<IEntityDeclarationSyntax> entities)
    {
        foreach (var entity in entities)
            Resolve(entity);
    }

    private void Resolve(IEntityDeclarationSyntax entity)
    {
        switch (entity)
        {
            default:
                throw ExhaustiveMatch.Failed(entity);
            case ITraitDeclarationSyntax syn:
                ResolveSupertypes(syn);
                break;
            case IClassDeclarationSyntax syn:
                Resolve(syn);
                break;
            case IMethodDeclarationSyntax syn:
                Resolve(syn);
                break;
            case IConstructorDeclarationSyntax syn:
                Resolve(syn);
                break;
            case IFunctionDeclarationSyntax syn:
                Resolve(syn);
                break;
            case IInvocableDeclarationSyntax syn:
                ResolveBody(syn);
                break;
            case IFieldDeclarationSyntax syn:
                ResolveInitializer(syn);
                break;
        }
    }

    private void ResolveSupertypes(ITypeDeclarationSyntax type)
    {
        // TODO error for duplicates
        foreach (var supertype in type.SupertypeNames)
            if (supertype.ReferencedSymbol.Result is not ObjectTypeSymbol)
                diagnostics.Add(OtherSemanticError.SupertypeMustBeClassOrTrait(type.File, type.Name, supertype));
    }

    private void ResolveAttributes(IFunctionDeclarationSyntax func)
    {
        if (!func.Attributes.Any())
            return;

        var typeResolver = new TypeResolver(func.File, diagnostics);
        foreach (var attribute in func.Attributes)
        {
            _ = typeResolver.EvaluateAttribute(attribute.TypeName);
            var referencedTypeSymbol = attribute.TypeName.ReferencedSymbol.Result;
            if (referencedTypeSymbol is null)
                continue;

            var noArgConstructor = symbolTrees.Children(referencedTypeSymbol)
                                    .OfType<ConstructorSymbol>().SingleOrDefault(c => c.Arity == 0);
            if (noArgConstructor is null)
                diagnostics.Add(NameBindingError.CouldNotBindName(func.File, attribute.TypeName.Span));
        }
    }

    private void Resolve(IClassDeclarationSyntax @class)
    {
        if (@class.BaseTypeName is not null)
            // TODO error for duplicates
            if (@class.BaseTypeName.ReferencedSymbol.Result is not ObjectTypeSymbol { DeclaresType.IsClass: true })
                diagnostics.Add(OtherSemanticError.BaseTypeMustBeClass(@class.File, @class.Name, @class.BaseTypeName));

        ResolveSupertypes(@class);
    }

    private void Resolve(IMethodDeclarationSyntax method)
    {
        var concreteClass = method.DeclaringType is IClassDeclarationSyntax { IsAbstract: false };
        if (concreteClass && method is IAbstractMethodDeclarationSyntax)
            diagnostics.Add(OtherSemanticError.AbstractMethodNotInAbstractClass(method.File, method.Span, method.Name));

        var symbol = method.Symbol.Result;

        var inConstClass = method.DeclaringType.Symbol.Result.DeclaresType.IsConst;
        var selfParameterType = symbol.SelfParameterType;
        var selfCapability = ((ReferenceType)selfParameterType.Type).Capability;
        if (inConstClass && !(selfCapability.IsConstant || selfCapability == ReferenceCapability.Identity))
            diagnostics.Add(TypeError.ConstClassSelfParameterCannotHaveCapability(method.File, method.SelfParameter));

        ResolveBody(method);
    }

    private void Resolve(IConstructorDeclarationSyntax constructor)
    {
        if (constructor.SelfParameter.IsLentBinding)
            diagnostics.Add(OtherSemanticError.LentConstructorSelf(constructor.File, constructor.SelfParameter));

        ResolveBody(constructor);
    }

    private void Resolve(IFunctionDeclarationSyntax func)
    {
        ResolveAttributes(func);
        ResolveBody(func);
    }

    private void ResolveBody(IInvocableDeclarationSyntax declaration)
    {
        switch (declaration)
        {
            default:
                throw ExhaustiveMatch.Failed(declaration);
            case IFunctionDeclarationSyntax function:
            {
                var resolver = new BasicBodyAnalyzer(function, symbolTreeBuilder, symbolTrees, stringSymbol, diagnostics,
                    function.Symbol.Result.ReturnType);
                resolver.ResolveTypes(function.Body);
                break;
            }
            case IAssociatedFunctionDeclarationSyntax associatedFunction:
            {
                var resolver = new BasicBodyAnalyzer(associatedFunction, symbolTreeBuilder, symbolTrees,
                    stringSymbol, diagnostics,
                    associatedFunction.Symbol.Result.ReturnType);
                resolver.ResolveTypes(associatedFunction.Body);
                break;
            }
            case IConcreteMethodDeclarationSyntax method:
            {
                var resolver = new BasicBodyAnalyzer(method, symbolTreeBuilder,
                    symbolTrees, stringSymbol, diagnostics, method.Symbol.Result.ReturnType);
                resolver.ResolveTypes(method.Body);
                break;
            }
            case IAbstractMethodDeclarationSyntax _:
                // has no body, so nothing to resolve
                break;
            case IConstructorDeclarationSyntax constructor:
            {
                ReturnType returnType = new ReturnType(false, constructor.SelfParameter.Symbol.Result.DataType);
                var resolver = new BasicBodyAnalyzer(constructor, symbolTreeBuilder, symbolTrees, stringSymbol, diagnostics, returnType);
                resolver.ResolveTypes(constructor.Body);
                break;
            }
        }
    }

    private void ResolveInitializer(IFieldDeclarationSyntax field)
    {
        if (field.Initializer is not null)
        {
            var resolver = new BasicBodyAnalyzer(field, symbolTreeBuilder, symbolTrees, stringSymbol, diagnostics);
            resolver.CheckFieldInitializerType(field.Initializer, field.Symbol.Result.DataType);
        }
    }
}
