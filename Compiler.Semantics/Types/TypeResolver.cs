using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

/// <summary>
/// Analyzes an <see cref="ITypeSyntax" /> to evaluate which type it refers to.
/// </summary>
public class TypeResolver
{
    private readonly CodeFile file;
    private readonly Diagnostics diagnostics;
    private readonly ITypeSymbolBuilder? typeSymbolBuilder;

    public TypeResolver(CodeFile file, Diagnostics diagnostics)
    {
        this.file = file;
        this.diagnostics = diagnostics;
    }

    public TypeResolver(CodeFile file, Diagnostics diagnostics, ITypeSymbolBuilder typeSymbolBuilder)
    {
        this.file = file;
        this.diagnostics = diagnostics;
        this.typeSymbolBuilder = typeSymbolBuilder;
    }

    [return: NotNullIfNotNull(nameof(typeSyntax))]
    public DataType? Evaluate(ITypeSyntax? typeSyntax)
    {
        switch (typeSyntax)
        {
            default:
                throw ExhaustiveMatch.Failed(typeSyntax);
            case null:
                return null;
            case ISimpleTypeNameSyntax syn:
                return ResolveType(syn, isAttribute: false, FixedList<DataType>.Empty, CreateType);
            case IParameterizedTypeSyntax syn:
                var typeArguments = Evaluate(syn.TypeArguments);
                return ResolveType(syn, isAttribute: false, typeArguments, CreateType);
            case ICapabilityTypeSyntax referenceCapability:
            {
                var capability = referenceCapability.Capability.Declared.ToReferenceCapability();
                var type = Evaluate(referenceCapability.Referent, isAttribute: false, capability);
                if (capability.AllowsWrite && type is ObjectType { IsConstType: true } objectType)
                    diagnostics.Add(TypeError.CannotApplyCapabilityToConstantType(file, referenceCapability, capability,
                        objectType.DeclaredType));
                return referenceCapability.NamedType = type;
            }
            case IOptionalTypeSyntax syn:
            {
                var referent = Evaluate(syn.Referent);
                return syn.NamedType = new OptionalType(referent);
            }
            case IFunctionTypeSyntax syn:
            {
                var parameterTypes = syn.Parameters.Select(Evaluate).ToFixedList();
                var returnType = Evaluate(syn.Return);
                return syn.NamedType = new FunctionType(parameterTypes, returnType);
            }
        }

        static DataType CreateType(TypeSymbol symbol, FixedList<DataType> typeArguments)
        {
            var type = symbol switch
            {
                // TODO WithoutWrite() is a hack for handling `Any`
                PrimitiveTypeSymbol sym => sym.DeclaresType.WithoutWrite(),
                ObjectTypeSymbol sym => sym.DeclaresType.WithRead(typeArguments),
                GenericParameterTypeSymbol sym => sym.DeclaresType,
                _ => throw ExhaustiveMatch.Failed(symbol)
            };
            return type;
        }
    }

    private DataType Evaluate(ITypeSyntax typeSyntax, bool isAttribute, ReferenceCapability capability)
    {
        switch (typeSyntax)
        {
            default:
                throw ExhaustiveMatch.Failed(typeSyntax);
            case ISimpleTypeNameSyntax syn:
                _ = EvaluateBareType(syn, isAttribute, capability);
                return syn.NamedType!;
            case IParameterizedTypeSyntax syn:
                _ = EvaluateBareType(syn, isAttribute, capability);
                return syn.NamedType!;
            case ICapabilityTypeSyntax _:
            case IOptionalTypeSyntax _:
            case IFunctionTypeSyntax _:
                throw new NotImplementedException("Report error about incorrect type expression.");
        }
    }

    private ParameterType Evaluate(IParameterTypeSyntax syn)
    {
        var referent = Evaluate(syn.Referent);
        return new ParameterType(syn.IsLent, referent);
    }

    private ReturnType Evaluate(IReturnTypeSyntax syn)
    {
        var referent = Evaluate(syn.Referent);
        return new ReturnType(syn.IsLent, referent);
    }

    /// <summary>
    /// Evaluate a type that should not have any reference capability.
    /// </summary>
    /// <remarks>This is used for new expressions and base types. It assigns an `id` reference
    /// capability to the type.</remarks>
    // TODO should this return `BareType`?
    public DataType EvaluateBareType(ITypeNameSyntax typeSyntax)
        => EvaluateBareType(typeSyntax, isAttribute: false, capability: null);

    // TODO should this return `BareType`?
    public DataType EvaluateAttribute(ITypeNameSyntax typeSyntax)
        => EvaluateBareType(typeSyntax, isAttribute: true, capability: null);

    // TODO should this return `BareType`?
    private DataType EvaluateBareType(
        ITypeNameSyntax typeSyntax,
        bool isAttribute,
        ReferenceCapability? capability)
    {
        return typeSyntax switch
        {
            ISimpleTypeNameSyntax syn => ResolveType(syn, isAttribute, FixedList<DataType>.Empty, CreateType),
            IParameterizedTypeSyntax syn
                => ResolveType(syn, isAttribute, Evaluate(syn.TypeArguments), CreateType),
            _ => throw ExhaustiveMatch.Failed(typeSyntax)
        };

        DataType CreateType(
            TypeSymbol symbol,
            FixedList<DataType> typeArguments)
        {
            switch (symbol)
            {
                default:
                    throw ExhaustiveMatch.Failed(symbol);
                case PrimitiveTypeSymbol sym:
                    var type = sym.DeclaresType;
                    // TODO Hack to handle `Any`
                    if (type is ReferenceType referenceType)
                    {
                        // If capability not provided, then this is for a constructor or something
                        // and reading the value doesn't matter, it exists to name the type.
                        capability ??= ReferenceCapability.Identity;
                        // Compatibility of the capability with the type is not checked here. That
                        // is done on the capability type syntax.
                        type = referenceType.With(capability);
                    }
                    return type;
                case ObjectTypeSymbol sym:
                    var declaredObjectType = sym.DeclaresType;
                    // If capability not provided, then this is for a constructor or something
                    // and reading the value doesn't matter, it exists to name the type.
                    capability ??= ReferenceCapability.Identity;
                    // Compatibility of the capability with the type is not checked here. That
                    // is done on the capability type syntax.
                    return declaredObjectType.With(capability, typeArguments);
                case GenericParameterTypeSymbol sym:
                    if (capability is not null)
                        diagnostics.Add(TypeError.CapabilityAppliedToTypeParameter(file, typeSyntax));
                    return sym.DeclaresType;
            }
        }
    }

    private FixedList<DataType> Evaluate(IEnumerable<ITypeSyntax> types)
        => types.Select(t => Evaluate(t)).ToFixedList();

    private DataType ResolveType(
        ITypeNameSyntax typeName,
        bool isAttribute,
        FixedList<DataType> typeArguments,
        Func<TypeSymbol, FixedList<DataType>, DataType> createType)
    {
        var symbols = typeName.LookupInContainingScope(withAttributeSuffix: false).Select(EnsureBuilt).ToFixedList();
        if (isAttribute && !symbols.Any())
            symbols = typeName.LookupInContainingScope(withAttributeSuffix: true).Select(EnsureBuilt).ToFixedList();
        switch (symbols.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindName(file, typeName.Span));
                typeName.ReferencedSymbol.Fulfill(null);
                return typeName.NamedType = DataType.Unknown;
            case 1:
                var symbol = symbols.Single();
                typeName.ReferencedSymbol.Fulfill(symbol);
                return typeName.NamedType = createType(symbol, typeArguments);
            default:
                diagnostics.Add(NameBindingError.AmbiguousName(file, typeName.Span));
                typeName.ReferencedSymbol.Fulfill(null);
                return typeName.NamedType = DataType.Unknown;
        }
    }

    private TypeSymbol EnsureBuilt(IPromise<TypeSymbol> promise)
    {
        if (promise.IsFulfilled) return promise.Result;
        if (typeSymbolBuilder is null)
            throw new InvalidOperationException("All type symbols should already be built");
        return typeSymbolBuilder.Build(promise);
    }

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
    public ObjectType EvaluateMethodSelfParameterType(
        DeclaredObjectType objectType,
        IReferenceCapabilitySyntax capability,
        FixedList<DataType> typeArguments)
        => objectType.With(capability.Declared.ToReferenceCapability(), typeArguments);

    public ObjectType EvaluateConstructorSelfParameterType(
        DeclaredObjectType objectType,
        IReferenceCapabilitySyntax capability,
        FixedList<DataType> typeArguments)
    {
        ReferenceCapability referenceCapability;
        switch (capability.Declared)
        {
            case DeclaredReferenceCapability.ReadOnly:
                referenceCapability = ReferenceCapability.InitReadOnly;
                break;
            case DeclaredReferenceCapability.Mutable:
                referenceCapability = ReferenceCapability.InitMutable;
                break;
            case DeclaredReferenceCapability.Isolated:
                diagnostics.Add(TypeError.InvalidConstructorSelfParameterCapability(file, capability));
                referenceCapability = ReferenceCapability.InitMutable;
                break;
            case DeclaredReferenceCapability.Constant:
            case DeclaredReferenceCapability.Identity:
                diagnostics.Add(TypeError.InvalidConstructorSelfParameterCapability(file, capability));
                referenceCapability = ReferenceCapability.InitReadOnly;
                break;
            default:
                throw ExhaustiveMatch.Failed(capability.Declared);
        }

        return objectType.With(referenceCapability, typeArguments);
    }
}
