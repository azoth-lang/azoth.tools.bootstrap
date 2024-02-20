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
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
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
    private readonly Pseudotype? selfType;
    private readonly ITypeSymbolBuilder? typeSymbolBuilder;

    public TypeResolver(CodeFile file, Diagnostics diagnostics, Pseudotype? selfType)
    {
        this.file = file;
        this.diagnostics = diagnostics;
        this.selfType = selfType;
    }

    public TypeResolver(
        CodeFile file,
        Diagnostics diagnostics,
        Pseudotype? selfType,
        ITypeSymbolBuilder typeSymbolBuilder)
    {
        this.file = file;
        this.diagnostics = diagnostics;
        this.selfType = selfType;
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
                return ResolveType(syn, isAttribute: false, FixedList.Empty<DataType>(), CreateType);
            case IParameterizedTypeSyntax syn:
                var typeArguments = Evaluate(syn.TypeArguments);
                return ResolveType(syn, isAttribute: false, typeArguments, CreateType);
            case ICapabilityTypeSyntax referenceCapability:
            {
                var capability = referenceCapability.Capability.Declared.ToReferenceCapability();
                var type = Evaluate(referenceCapability.Referent, isAttribute: false, capability);
                if (capability.AllowsWrite && type is ReferenceType { IsDeclaredConst: true } referenceType)
                    diagnostics.Add(TypeError.CannotApplyCapabilityToConstantType(file, referenceCapability, capability,
                        referenceType.DeclaredType));
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
            case ICapabilityViewpointTypeSyntax syn:
            {
                var capability = syn.Capability.Declared.ToReferenceCapability();
                var type = Evaluate(syn.Referent);
                if (type is GenericParameterType genericParameterType)
                    return syn.NamedType = CapabilityViewpointType.Create(capability, genericParameterType);

                diagnostics.Add(TypeError.CapabilityViewpointNotAppliedToTypeParameter(file, syn));
                return syn.NamedType = type;
            }
            case ISelfViewpointTypeSyntax syn:
            {
                var referentType = Evaluate(syn.Referent);
                if (selfType is ReferenceType { Capability: var capability }
                        && referentType is GenericParameterType genericParameterType)
                    return syn.NamedType = CapabilityViewpointType.Create(capability, genericParameterType);

                if (selfType is ObjectTypeConstraint { Capability: var capabilityConstraint })
                    return syn.NamedType = new SelfViewpointType(capabilityConstraint, referentType);

                if (selfType is not (ReferenceType or ObjectTypeConstraint))
                    diagnostics.Add(TypeError.SelfViewpointNotAvailable(file, syn));

                if (referentType is not GenericParameterType)
                    diagnostics.Add(TypeError.SelfViewpointNotAppliedToTypeParameter(file, syn));

                return syn.NamedType = referentType;
            }
        }

        static DataType CreateType(TypeSymbol symbol, IFixedList<DataType> typeArguments)
        {
            return symbol switch
            {
                PrimitiveTypeSymbol sym => sym.DeclaresType.WithRead(typeArguments),
                ObjectTypeSymbol sym => sym.DeclaresType.WithRead(typeArguments),
                GenericParameterTypeSymbol sym => sym.DeclaresType,
                EmptyTypeSymbol sym => sym.DeclaresType,
                _ => throw ExhaustiveMatch.Failed(symbol)
            };
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
            case IViewpointTypeSyntax _:
                throw new NotImplementedException("Report error about incorrect type expression.");
        }
    }

    private Parameter Evaluate(IParameterTypeSyntax syn)
    {
        var referent = Evaluate(syn.Referent);
        return new Parameter(syn.IsLent, referent);
    }

    private Return Evaluate(IReturnTypeSyntax syn)
    {
        var referent = Evaluate(syn.Referent);
        return new Return(referent);
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
            ISimpleTypeNameSyntax syn => ResolveType(syn, isAttribute, FixedList.Empty<DataType>(), CreateType),
            IParameterizedTypeSyntax syn
                => ResolveType(syn, isAttribute, Evaluate(syn.TypeArguments), CreateType),
            _ => throw ExhaustiveMatch.Failed(typeSyntax)
        };

        DataType CreateType(
            TypeSymbol symbol,
            IFixedList<DataType> typeArguments)
        {
            switch (symbol)
            {
                default:
                    throw ExhaustiveMatch.Failed(symbol);
                case PrimitiveTypeSymbol sym:
                {
                    var declaredType = sym.DeclaresType;
                    // If capability not provided, then this is for a constructor or something
                    // and reading the value doesn't matter, it exists to name the type.
                    capability ??= ReferenceCapability.Identity;
                    // Compatibility of the capability with the type is not checked here. That
                    // is done on the capability type syntax.
                    return declaredType.With(capability, typeArguments);
                }
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
                case EmptyTypeSymbol sym:
                    if (capability is not null)
                        diagnostics.Add(TypeError.CapabilityAppliedToEmptyType(file, typeSyntax));
                    return sym.DeclaresType;
            }
        }
    }

    private IFixedList<DataType> Evaluate(IEnumerable<ITypeSyntax> types)
        => types.Select(t => Evaluate(t)).ToFixedList();

    private DataType ResolveType(
        ITypeNameSyntax typeName,
        bool isAttribute,
        IFixedList<DataType> typeArguments,
        Func<TypeSymbol, IFixedList<DataType>, DataType> createType)
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
}
