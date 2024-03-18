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
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
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
        => typeSyntax is not null ? Evaluate(typeSyntax, mustBeConstructable: false) : null;

    private DataType Evaluate(ITypeSyntax typeSyntax, bool mustBeConstructable)
    {
        switch (typeSyntax)
        {
            default:
                throw ExhaustiveMatch.Failed(typeSyntax);
            case ISimpleTypeNameSyntax syn:
                return ResolveType(syn, isAttribute: false, FixedList.Empty<DataType>(), CreateType);
            case IGenericTypeNameSyntax syn:
                var typeArguments = Evaluate(syn.TypeArguments, mustBeConstructable);
                return ResolveType(syn, isAttribute: false, typeArguments, CreateType);
            case ICapabilityTypeSyntax referenceCapability:
            {
                var capability = referenceCapability.Capability.Declared.ToCapability();
                var type = Evaluate(capability, referenceCapability.Referent, isAttribute: false, mustBeConstructable: mustBeConstructable);
                if (capability.AllowsWrite && type is ReferenceType { IsDeclaredConst: true } referenceType)
                    diagnostics.Add(TypeError.CannotApplyCapabilityToConstantType(file, referenceCapability, capability,
                        referenceType.DeclaredType));
                return referenceCapability.NamedType = type;
            }
            case IOptionalTypeSyntax syn:
            {
                var referent = Evaluate(syn.Referent, mustBeConstructable);
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
                var capability = syn.Capability.Declared.ToCapability();
                var type = Evaluate(syn.Referent, mustBeConstructable);
                if (type is GenericParameterType genericParameterType)
                    return syn.NamedType = CapabilityViewpointType.Create(capability, genericParameterType);

                diagnostics.Add(TypeError.CapabilityViewpointNotAppliedToTypeParameter(file, syn));
                return syn.NamedType = type;
            }
            case ISelfViewpointTypeSyntax syn:
            {
                var referentType = Evaluate(syn.Referent, mustBeConstructable);
                if (selfType is ReferenceType { Capability: var capability }
                        && referentType is GenericParameterType genericParameterType)
                    return syn.NamedType = CapabilityViewpointType.Create(capability, genericParameterType);

                if (selfType is CapabilityTypeConstraint { Capability: var capabilityConstraint })
                    return syn.NamedType = new SelfViewpointType(capabilityConstraint, referentType);

                if (selfType is not (ReferenceType or CapabilityTypeConstraint))
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
                UserTypeSymbol sym => sym.DeclaresType.WithRead(typeArguments),
                GenericParameterTypeSymbol sym => sym.DeclaresType,
                EmptyTypeSymbol sym => sym.DeclaresType,
                _ => throw ExhaustiveMatch.Failed(symbol)
            };
        }
    }

    private DataType Evaluate(Capability capability, ITypeSyntax typeSyntax, bool isAttribute, bool mustBeConstructable)
    {
        switch (typeSyntax)
        {
            default:
                throw ExhaustiveMatch.Failed(typeSyntax);
            case ISimpleTypeNameSyntax syn:
                _ = Evaluate(capability, syn, isAttribute, mustBeConstructable);
                return syn.NamedType!;
            case IGenericTypeNameSyntax syn:
                _ = Evaluate(capability, syn, isAttribute, mustBeConstructable);
                return syn.NamedType!;
            case ICapabilityTypeSyntax _:
            case IOptionalTypeSyntax _:
            case IFunctionTypeSyntax _:
            case IViewpointTypeSyntax _:
                throw new NotImplementedException("Report error about incorrect type expression.");
        }
    }

    private DataType Evaluate(
        Capability capability,
        ITypeNameSyntax typeSyntax,
        bool isAttribute,
        bool mustBeConstructable)
    {
        return typeSyntax switch
        {
            ISimpleTypeNameSyntax syn => ResolveType(syn, isAttribute, FixedList.Empty<DataType>(), CreateType),
            IGenericTypeNameSyntax syn
                => ResolveType(syn, isAttribute, Evaluate(syn.TypeArguments, mustBeConstructable), CreateType),
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
                    // Compatibility of the capability with the type is not checked here. That
                    // is done on the capability type syntax.
                    return declaredType.With(capability, typeArguments);
                }
                case UserTypeSymbol sym:
                    var declaredObjectType = sym.DeclaresType;
                    // Compatibility of the capability with the type is not checked here. That
                    // is done on the capability type syntax.
                    return declaredObjectType.With(capability, typeArguments);
                case GenericParameterTypeSymbol sym:
                    diagnostics.Add(TypeError.CapabilityAppliedToTypeParameter(file, typeSyntax));
                    return sym.DeclaresType;
                case EmptyTypeSymbol sym:
                    diagnostics.Add(TypeError.CapabilityAppliedToEmptyType(file, typeSyntax));
                    return sym.DeclaresType;
            }
        }
    }

    private Parameter Evaluate(IParameterTypeSyntax syn)
    {
        var referent = Evaluate(syn.Referent, mustBeConstructable: false);
        return new Parameter(syn.IsLent, referent);
    }

    private Return Evaluate(IReturnTypeSyntax syn)
    {
        var referent = Evaluate(syn.Referent, mustBeConstructable: false);
        return new Return(referent);
    }

    /// <summary>
    /// Evaluate a type that does not have any reference capability.
    /// </summary>
    /// <remarks>This is used for new expressions.</remarks>
    public BareType? EvaluateConstructableBareType(ITypeNameSyntax typeSyntax)
        => EvaluateBareType(typeSyntax, isAttribute: false);

    public BareReferenceType? Evaluate(ISupertypeNameSyntax typeSyntax)
    {
        var symbols = typeSyntax.LookupInContainingScope().Select(EnsureBuilt).ToFixedList();
        var typeArguments = Evaluate(typeSyntax.TypeArguments, mustBeConstructable: false);
        switch (symbols.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindName(file, typeSyntax.Span));
                typeSyntax.ReferencedSymbol.Fulfill(null);
                return typeSyntax.NamedType.Fulfill(null);
            case 1:
                var symbol = symbols.Single();
                typeSyntax.ReferencedSymbol.Fulfill(symbol);
                var declaredObjectType = symbol.DeclaresType;
                var type = CheckTypeArgumentsAreConstructable(declaredObjectType.With(typeArguments), typeSyntax) as BareReferenceType;
                return typeSyntax.NamedType.Fulfill(type);
            default:
                diagnostics.Add(NameBindingError.AmbiguousName(file, typeSyntax.Span));
                typeSyntax.ReferencedSymbol.Fulfill(null);
                return typeSyntax.NamedType.Fulfill(null);
        }
    }

    public BareType? EvaluateAttribute(ITypeNameSyntax typeSyntax)
        => EvaluateBareType(typeSyntax, isAttribute: true);

    private BareType? EvaluateBareType(
        ITypeNameSyntax typeSyntax,
        bool isAttribute)
    {
        return typeSyntax switch
        {
            ISimpleTypeNameSyntax syn => ResolveType(syn, isAttribute, FixedList.Empty<DataType>(), CreateType),
            IGenericTypeNameSyntax syn
                => ResolveType(syn, isAttribute, Evaluate(syn.TypeArguments, mustBeConstructable: true), CreateType),
            _ => throw ExhaustiveMatch.Failed(typeSyntax)
        };

        BareType? CreateType(
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
                    return declaredType.With(typeArguments);
                }
                case UserTypeSymbol sym:
                    var declaredObjectType = sym.DeclaresType;
                    return CheckTypeArgumentsAreConstructable(declaredObjectType.With(typeArguments), typeSyntax);
                case GenericParameterTypeSymbol _:
                    diagnostics.Add(TypeError.TypeParameterCannotBeUsedHere(file, typeSyntax));
                    return null;
                case EmptyTypeSymbol _:
                    diagnostics.Add(TypeError.EmptyTypeCannotBeUsedHere(file, typeSyntax));
                    return null;
            }
        }
    }

    private IFixedList<DataType> Evaluate(IEnumerable<ITypeSyntax> types, bool mustBeConstructable)
        => types.Select(t => Evaluate(t, mustBeConstructable)).ToFixedList();

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

    private BareType? ResolveType(
        ITypeNameSyntax typeName,
        bool isAttribute,
        IFixedList<DataType> typeArguments,
        Func<TypeSymbol, IFixedList<DataType>, BareType?> createType)
    {
        var symbols = typeName.LookupInContainingScope(withAttributeSuffix: false).Select(EnsureBuilt).ToFixedList();
        if (isAttribute && !symbols.Any())
            symbols = typeName.LookupInContainingScope(withAttributeSuffix: true).Select(EnsureBuilt).ToFixedList();
        switch (symbols.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindName(file, typeName.Span));
                typeName.ReferencedSymbol.Fulfill(null);
                typeName.NamedType = DataType.Unknown;
                return null;
            case 1:
                var symbol = symbols.Single();
                typeName.ReferencedSymbol.Fulfill(symbol);
                var bareType = createType(symbol, typeArguments);
                typeName.NamedType = (DataType?)bareType?.With(Capability.Identity) ?? DataType.Unknown;
                return bareType;
            default:
                diagnostics.Add(NameBindingError.AmbiguousName(file, typeName.Span));
                typeName.ReferencedSymbol.Fulfill(null);
                typeName.NamedType = DataType.Unknown;
                return null;
        }
    }

    private TSymbol EnsureBuilt<TSymbol>(IPromise<TSymbol> promise)
        where TSymbol : TypeSymbol
    {
        if (promise.IsFulfilled) return promise.Result;
        if (typeSymbolBuilder is null)
            throw new InvalidOperationException("All type symbols should already be built");
        return typeSymbolBuilder.Build(promise);
    }

    private BareType? CheckTypeArgumentsAreConstructable(BareType type, ISyntax typeSyntax)
    {
        var constructable = true;
        foreach (var (param, arg) in type.GenericParameterArguments)
            constructable &= CheckTypeArgumentIsConstructable(param, arg, typeSyntax);

        return constructable ? type : null;
    }

    private bool CheckTypeArgumentIsConstructable(GenericParameter param, DataType arg, ISyntax typeSyntax)
    {
        switch (arg)
        {
            case CapabilityType capabilityType:
                if (!param.Constraint.IsAssignableFrom(capabilityType.Capability))
                {
                    diagnostics.Add(TypeError.CapabilityNotCompatibleWithConstraint(file, typeSyntax, param, capabilityType));
                    return false;
                }
                break;
            case OptionalType optionalType:
                return CheckTypeArgumentIsConstructable(param, optionalType.Referent, typeSyntax);
            case GenericParameterType genericParameterType:
                if (!param.Constraint.IsAssignableFrom(genericParameterType.Parameter.Constraint))
                {
                    diagnostics.Add(TypeError.CapabilityNotCompatibleWithConstraint(file, typeSyntax, param, genericParameterType));
                    return false;
                }
                break;
            case CapabilityViewpointType viewpointType:
                // TODO handle capabilities on the generic parameter
                if (!param.Constraint.IsAssignableFrom(viewpointType.Capability))
                {
                    diagnostics.Add(TypeError.CapabilityNotCompatibleWithConstraint(file, typeSyntax, param, viewpointType));
                    return false;
                }
                break;
            case SelfViewpointType selfViewpointType:
                // TODO handle capabilities on the referent
                if (!param.Constraint.IsAssignableFrom(selfViewpointType.Capability))
                {
                    diagnostics.Add(TypeError.CapabilityNotCompatibleWithConstraint(file, typeSyntax, param, selfViewpointType));
                    return false;
                }
                break;
            case EmptyType _:
            case UnknownType _:
            case FunctionType _:
            case ConstValueType _:
                // ignore
                break;
            default:
                throw ExhaustiveMatch.Failed(arg);
        }
        return true;
    }
}
