using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    public TypeResolver(CodeFile file, Diagnostics diagnostics, Pseudotype? selfType)
    {
        this.file = file;
        this.diagnostics = diagnostics;
        this.selfType = selfType;
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
                return ResolveType(syn, FixedList.Empty<DataType>(), CreateType);
            case IGenericTypeNameSyntax syn:
                var typeArguments = Evaluate(syn.TypeArguments, mustBeConstructable);
                return ResolveType(syn, typeArguments, CreateType);
            case IQualifiedTypeNameSyntax syn:
                throw new NotImplementedException("IQualifiedTypeNameSyntax");
            case ICapabilityTypeSyntax referenceCapability:
            {
                var capability = referenceCapability.Capability.Declared.ToCapability();
                var type = Evaluate(capability, referenceCapability.Referent, mustBeConstructable: mustBeConstructable);
                // Diagnostic moved to semantic tree
                //if (capability.AllowsWrite && type is ReferenceType { IsDeclaredConst: true } referenceType)
                //    diagnostics.Add(TypeError.CannotApplyCapabilityToConstantType(file, referenceCapability, capability,
                //        referenceType.DeclaredType));
                // Type already set by SemanticsApplier
                if (referenceCapability.NamedType != type)
                    throw new UnreachableException("Types should match.");
                return type;
            }
            case IOptionalTypeSyntax syn:
            {
                var referent = Evaluate(syn.Referent, mustBeConstructable);
                var optionalType = OptionalType.Create(referent);
                // Type already set by SemanticsApplier
                if (syn.NamedType != optionalType)
                    throw new UnreachableException("Types should match.");
                return optionalType;
            }
            case IFunctionTypeSyntax syn:
            {
                var parameterTypes = syn.Parameters.Select(Evaluate).ToFixedList();
                var returnType = Evaluate(syn.Return);
                var functionType = new FunctionType(parameterTypes, returnType);
                // Type already set by SemanticsApplier
                if (syn.NamedType != functionType)
                    throw new UnreachableException("Types should match.");
                return functionType;
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
                if (selfType is CapabilityType { Capability: var capability }
                        && referentType is GenericParameterType genericParameterType)
                    return syn.NamedType = CapabilityViewpointType.Create(capability, genericParameterType);

                if (selfType is CapabilityTypeConstraint { Capability: var capabilityConstraint })
                    return syn.NamedType = SelfViewpointType.Create(capabilityConstraint, referentType);

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
                GenericParameterTypeSymbol sym => sym.Type,
                EmptyTypeSymbol sym => sym.Type,
                _ => throw ExhaustiveMatch.Failed(symbol)
            };
        }
    }

    private DataType Evaluate(Capability capability, ITypeSyntax typeSyntax, bool mustBeConstructable)
    {
        switch (typeSyntax)
        {
            default:
                throw ExhaustiveMatch.Failed(typeSyntax);
            case ISimpleTypeNameSyntax syn:
                return Evaluate(capability, syn, mustBeConstructable);
            case IGenericTypeNameSyntax syn:
                return Evaluate(capability, syn, mustBeConstructable);
            case IQualifiedTypeNameSyntax syn:
                throw new NotImplementedException("IQualifiedTypeNameSyntax");
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
        bool mustBeConstructable)
    {
        return typeSyntax switch
        {
            ISimpleTypeNameSyntax syn => ResolveType(syn, FixedList.Empty<DataType>(), CreateType),
            IGenericTypeNameSyntax syn
                => ResolveType(syn, Evaluate(syn.TypeArguments, mustBeConstructable), CreateType),
            IQualifiedTypeNameSyntax syn => throw new NotImplementedException("IQualifiedTypeNameSyntax"),
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
                    // Diagnostic moved to semantic tree
                    //diagnostics.Add(TypeError.CapabilityAppliedToTypeParameter(file, typeSyntax));
                    return sym.Type;
                case EmptyTypeSymbol sym:
                    diagnostics.Add(TypeError.CapabilityAppliedToEmptyType(file, typeSyntax));
                    return sym.Type;
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

    private IFixedList<DataType> Evaluate(IEnumerable<ITypeSyntax> types, bool mustBeConstructable)
        => types.Select(t => Evaluate(t, mustBeConstructable)).ToFixedList();

    private DataType ResolveType(
        ITypeNameSyntax typeName,
        IFixedList<DataType> typeArguments,
        Func<TypeSymbol, IFixedList<DataType>, DataType> createType)
    {
        var symbols = typeName.LookupInContainingScope(withAttributeSuffix: false).Select(EnsureBuilt).ToFixedList();
        switch (symbols.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindName(file, typeName.Span));
                typeName.ReferencedSymbol.Fulfill(null);
                return typeName.NamedType = DataType.Unknown;
            case 1:
                var symbol = symbols.Single();
                if (typeName.ReferencedSymbol.IsFulfilled)
                {
                    // Symbol already fulfilled by SemanticsApplier
                    var existingSymbol = typeName.ReferencedSymbol.Result!;
                    if (symbol != existingSymbol)
                        throw new UnreachableException("Symbols should match");
                    // In this case NamedType should already be set too
                    return createType(symbol, typeArguments);
                }

                typeName.ReferencedSymbol.Fulfill(symbol);
                return typeName.NamedType = createType(symbol, typeArguments);
            default:
                diagnostics.Add(NameBindingError.AmbiguousName(file, typeName.Span));
                typeName.ReferencedSymbol.Fulfill(null);
                return typeName.NamedType = DataType.Unknown;
        }
    }

    private static TSymbol EnsureBuilt<TSymbol>(IPromise<TSymbol> promise)
        where TSymbol : TypeSymbol
    {
        if (promise.IsFulfilled)
            return promise.Result;
        throw new InvalidOperationException("All type symbols should already be built");
    }
}
