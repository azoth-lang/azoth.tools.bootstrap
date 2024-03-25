using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
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
    private readonly ISymbolTreeBuilder symbolTreeBuilder;
    private readonly SymbolForest symbolTrees;
    private readonly UserTypeSymbol? stringSymbol;
    private readonly UserTypeSymbol? rangeSymbol;
    private readonly Diagnostics diagnostics;

    private BasicAnalyzer(
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        UserTypeSymbol? stringSymbol,
        UserTypeSymbol? rangeSymbol,
        Diagnostics diagnostics)
    {
        this.symbolTreeBuilder = symbolTreeBuilder;
        this.symbolTrees = symbolTrees;
        this.stringSymbol = stringSymbol;
        this.rangeSymbol = rangeSymbol;
        this.diagnostics = diagnostics;
    }

    public static void Check(
        PackageSyntax<Package> package,
        UserTypeSymbol? stringSymbol,
        UserTypeSymbol? rangeSymbol)
    {
        // Analyze standard code (*.az)
        var analyzer = new BasicAnalyzer(package.SymbolTree, package.SymbolTrees, stringSymbol, rangeSymbol, package.Diagnostics);
        analyzer.Resolve(package.EntityDeclarations);

        // Analyze test code (*.azt)
        analyzer = new BasicAnalyzer(package.TestingSymbolTree, package.TestingSymbolTrees, stringSymbol, rangeSymbol, package.Diagnostics);
        analyzer.Resolve(package.TestingEntityDeclarations);
    }

    private void Resolve(IFixedSet<IEntityDeclarationSyntax> entities)
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
            case IClassDeclarationSyntax syn:
                Resolve(syn);
                break;
            case ITraitDeclarationSyntax syn:
                Resolve(syn);
                break;
            case IStructDeclarationSyntax syn:
                Resolve(syn);
                break;
            case IMethodDeclarationSyntax syn:
                Resolve(syn);
                break;
            case IConstructorDeclarationSyntax syn:
                Resolve(syn);
                break;
            case IInitializerDeclarationSyntax syn:
                Resolve(syn);
                break;
            case IFunctionDeclarationSyntax syn:
                Resolve(syn);
                break;
            case IAssociatedFunctionDeclarationSyntax syn:
                Resolve(syn);
                break;
            case IFieldDeclarationSyntax syn:
                Resolve(syn);
                break;
        }
    }

    private void ResolveSupertypes(ITypeDeclarationSyntax type)
    {
        // TODO error for duplicates
        foreach (var supertype in type.SupertypeNames)
            if (supertype.ReferencedSymbol.Result is null)
                diagnostics.Add(OtherSemanticError.SupertypeMustBeClassOrTrait(type.File, type.Name, supertype));
    }

    private void ResolveAttributes(IFunctionDeclarationSyntax func)
    {
        if (!func.Attributes.Any())
            return;

        var typeResolver = new TypeResolver(func.File, diagnostics, selfType: null);
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
        // Resolve base class
        if (@class.BaseTypeName is not null)
            // TODO error for duplicates
            if (@class.BaseTypeName.ReferencedSymbol.Result is not UserTypeSymbol { DeclaresType.IsClass: true })
                diagnostics.Add(OtherSemanticError.BaseTypeMustBeClass(@class.File, @class.Name, @class.BaseTypeName));

        ResolveSupertypes(@class);
        CheckSupertypesAreOutputSafe(@class, @class.AllSupertypeNames.ToFixedList());
        CheckSupertypesMaintainIndependence(@class);
    }

    private void CheckSupertypesAreOutputSafe(
        ITypeDeclarationSyntax typeDeclaration,
        IFixedList<ISupertypeNameSyntax> allSuperTypes)
    {
        var declaresType = typeDeclaration.Symbol.Result.DeclaresType;
        // TODO nested classes and traits need to be checked if nested inside of generic types
        if (!declaresType.IsGeneric)
            return;
        var nonwritableSelf = declaresType.IsDeclaredConst ? true : (bool?)null;
        foreach (var typeNameSyntax in allSuperTypes)
        {
            var type = typeNameSyntax.NamedType.Result;
            if (type is not null && !type.IsSupertypeOutputSafe(nonwritableSelf))
                diagnostics.Add(TypeError.SupertypeMustBeOutputSafe(typeDeclaration.File, typeNameSyntax));
        }
    }

    private void CheckSupertypesMaintainIndependence(
        ITypeDeclarationSyntax typeDeclaration)
    {
        var declaresType = typeDeclaration.Symbol.Result.DeclaresType;
        if (!declaresType.HasIndependentGenericParameters)
            return;

        if (typeDeclaration is IClassDeclarationSyntax { BaseTypeName: var baseTypeNameSyntax })
            if (baseTypeNameSyntax is not null
                && (!baseTypeNameSyntax.NamedType.Result?.SupertypeMaintainsIndependence(exact: true) ?? false))
                diagnostics.Add(TypeError.SupertypeMustMaintainIndependence(typeDeclaration.File, baseTypeNameSyntax));

        foreach (var typeNameSyntax in typeDeclaration.SupertypeNames)
            if (!typeNameSyntax.NamedType.Result?.SupertypeMaintainsIndependence(exact: false) ?? false)
                diagnostics.Add(TypeError.SupertypeMustMaintainIndependence(typeDeclaration.File, typeNameSyntax));
    }

    private void Resolve(IStructDeclarationSyntax @struct)
    {
        ResolveSupertypes(@struct);
        CheckSupertypesAreOutputSafe(@struct, @struct.SupertypeNames);
        CheckSupertypesMaintainIndependence(@struct);
    }

    private void Resolve(ITraitDeclarationSyntax trait)
    {
        ResolveSupertypes(trait);
        CheckSupertypesAreOutputSafe(trait, trait.SupertypeNames);
        CheckSupertypesMaintainIndependence(trait);
    }

    private void Resolve(IMethodDeclarationSyntax method)
    {
        var concreteClass = method.DeclaringType is IClassDeclarationSyntax { IsAbstract: false };
        if (concreteClass && method is IAbstractMethodDeclarationSyntax)
            diagnostics.Add(OtherSemanticError.AbstractMethodNotInAbstractClass(method.File, method.Span, method.Name));

        var symbol = method.Symbol.Result;

        var inConstClass = method.DeclaringType.Symbol.Result.DeclaresType.IsDeclaredConst;
        var selfParameterType = symbol.SelfParameterType;
        var selfType = selfParameterType.Type;
        if (inConstClass &&
           ((selfType is CapabilityType { Capability: var selfCapability } && selfCapability != Capability.Constant && selfCapability != Capability.Identity)
           || selfType is CapabilityTypeConstraint))
            diagnostics.Add(TypeError.ConstClassSelfParameterCannotHaveCapability(method.File, method.SelfParameter));

        CheckParameterAndReturnAreVarianceSafe(method);
        ResolveBody(method);
    }

    private void CheckParameterAndReturnAreVarianceSafe(IMethodDeclarationSyntax method)
    {
        // TODO do generic methods and functions need to be checked?

        var methodSymbol = method.Symbol.Result;
        // Only methods declared in generic types need checked
        if (methodSymbol.ContainingSymbol is not UserTypeSymbol { DeclaresType.IsGeneric: true })
            return;

        var nonwritableSelf = !method.SelfParameter.Capability.Constraint.AnyCapabilityAllowsWrite;

        // The `self` parameter does not get checked for variance safety. It will always operate on
        // the original type so it is safe.
        foreach (var parameter in method.Parameters)
        {
            var type = parameter.DataType.Result;
            if (!type.IsInputSafe(nonwritableSelf))
                diagnostics.Add(TypeError.ParameterMustBeInputSafe(method.File, parameter, type));
        }

        var returnType = methodSymbol.Return.Type;
        if (!returnType.IsOutputSafe(nonwritableSelf))
            diagnostics.Add(TypeError.ReturnTypeMustBeOutputSafe(method.File, method.Return!.Type, returnType));
    }

    private void Resolve(IConstructorDeclarationSyntax constructor)
    {
        if (constructor.SelfParameter.IsLentBinding)
            diagnostics.Add(OtherSemanticError.LentConstructorOrInitializerSelf(constructor.File, constructor.SelfParameter));

        // Constructors are like associated functions, so they don't need their parameters or
        // return type checked for variance safety.

        ResolveBody(constructor);
    }

    private void Resolve(IInitializerDeclarationSyntax initializer)
    {
        if (initializer.SelfParameter.IsLentBinding)
            diagnostics.Add(OtherSemanticError.LentConstructorOrInitializerSelf(initializer.File, initializer.SelfParameter));

        // Initializers are like associated functions, so they don't need their parameters or
        // return type checked for variance safety.

        ResolveBody(initializer);
    }

    private void Resolve(IFunctionDeclarationSyntax func)
    {
        ResolveAttributes(func);
        ResolveBody(func);
    }

    private void Resolve(IAssociatedFunctionDeclarationSyntax associatedFunction)
        // Associated functions aren't called from instances, so their type parameters always act as
        // if they were invariant. This means they don't need to be checked for variance safety.
        => ResolveBody(associatedFunction);

    private void ResolveBody(IInvocableDeclarationSyntax declaration)
    {
        switch (declaration)
        {
            default:
                throw ExhaustiveMatch.Failed(declaration);
            case IFunctionDeclarationSyntax function:
            {
                var resolver = new BasicBodyAnalyzer(function, symbolTreeBuilder, symbolTrees,
                    stringSymbol, rangeSymbol, diagnostics,
                    function.Symbol.Result.Return);
                resolver.ResolveTypes(function.Body);
                break;
            }
            case IAssociatedFunctionDeclarationSyntax associatedFunction:
            {
                var resolver = new BasicBodyAnalyzer(associatedFunction, symbolTreeBuilder, symbolTrees,
                    stringSymbol, rangeSymbol, diagnostics,
                    associatedFunction.Symbol.Result.Return);
                resolver.ResolveTypes(associatedFunction.Body);
                break;
            }
            case IConcreteMethodDeclarationSyntax method:
            {
                var resolver = new BasicBodyAnalyzer(method, symbolTreeBuilder,
                    symbolTrees, stringSymbol, rangeSymbol, diagnostics,
                    method.Symbol.Result.Return);
                resolver.ResolveTypes(method.Body);
                break;
            }
            case IAbstractMethodDeclarationSyntax _:
                // has no body, so nothing to resolve
                break;
            case IConstructorDeclarationSyntax constructor:
            {
                Return @return = new Return(constructor.SelfParameter.DataType.Result);
                var resolver = new BasicBodyAnalyzer(constructor, symbolTreeBuilder, symbolTrees,
                    stringSymbol, rangeSymbol, diagnostics, @return);
                resolver.ResolveTypes(constructor.Body);
                break;
            }
            case IInitializerDeclarationSyntax initializer:
            {
                Return @return = new Return(initializer.SelfParameter.DataType.Result);
                var resolver = new BasicBodyAnalyzer(initializer, symbolTreeBuilder, symbolTrees,
                    stringSymbol, rangeSymbol, diagnostics, @return);
                resolver.ResolveTypes(initializer.Body);
                break;
            }
        }
    }

    private void Resolve(IFieldDeclarationSyntax field)
    {
        var fieldSymbol = field.Symbol.Result;
        var type = fieldSymbol.Type;

        // Check variance safety. Only public fields need their safety checked. Effectively, they
        // have getters and setters. Private and protected fields are only accessed from within the
        // class where the exact type parameters are known, so they are always safe.
        if (field.AccessModifier.ToAccessModifier() >= AccessModifier.Public)
        {
            if (fieldSymbol.IsMutableBinding)
            {
                // Mutable bindings can be both read and written to, so they must be both input and output
                // safe (i.e. invariant). Self is nonwritable for the output case which is where
                // self writable matters.
                if (!type.IsInputAndOutputSafe(nonwriteableSelf: true))
                    diagnostics.Add(TypeError.VarFieldMustBeInputAndOutputSafe(field.File, field, type));
            }
            else
            {
                // Immutable bindings can only be read, so they must be output safe.
                if (!type.IsOutputSafe(nonwritableSelf: true))
                    diagnostics.Add(TypeError.LetFieldMustBeOutputSafe(field.File, field, type));
            }
        }

        // Fields must also maintain the independence of independent type parameters
        if (!type.FieldMaintainsIndependence())
            diagnostics.Add(TypeError.FieldMustMaintainIndependence(field.File, field, type));

        if (field.Initializer is not null)
        {
            var resolver = new BasicBodyAnalyzer(field, symbolTreeBuilder, symbolTrees,
                stringSymbol, rangeSymbol, diagnostics);
            resolver.CheckFieldInitializerType(field.Initializer, type);
        }
    }
}
