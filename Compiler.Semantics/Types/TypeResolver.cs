using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
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

    public TypeResolver(CodeFile file, Diagnostics diagnostics)
    {
        this.file = file;
        this.diagnostics = diagnostics;
    }

    [return: NotNullIfNotNull(nameof(typeSyntax))]
    public DataType? Evaluate(ITypeSyntax? typeSyntax, bool implicitRead)
    {
        // TODO it isn't clear that the implicitRead parameter makes sense
        switch (typeSyntax)
        {
            default:
                throw ExhaustiveMatch.Failed(typeSyntax);
            case null:
                return null;
            case ISimpleTypeNameSyntax syn:
                return ResolveType(syn, FixedList<DataType>.Empty, CreateType);
            case IParameterizedTypeSyntax syn:
                var typeArguments = Evaluate(syn.TypeArguments);
                return ResolveType(syn, typeArguments, CreateType);
            case ICapabilityTypeSyntax referenceCapability:
            {
                var capability = referenceCapability.Capability.Declared.ToReferenceCapability();
                var type = Evaluate(referenceCapability.ReferentType, capability);
                if (capability.AllowsWrite && type is ObjectType { IsConst: true } objectType)
                    diagnostics.Add(TypeError.CannotApplyCapabilityToConstantType(file, referenceCapability, capability,
                        objectType.DeclaredType));
                return referenceCapability.NamedType = type;
            }
            case IOptionalTypeSyntax optionalType:
            {
                var referent = Evaluate(optionalType.Referent, implicitRead);
                return optionalType.NamedType = new OptionalType(referent);
            }
        }

        DataType CreateType(TypeSymbol symbol, FixedList<DataType> typeArguments)
        {
            var type = symbol switch
            {
                PrimitiveTypeSymbol sym => sym.DeclaresType,
                ObjectTypeSymbol sym => implicitRead
                    ? sym.DeclaresType.WithRead(typeArguments)
                    : sym.DeclaresType.WithMutate(typeArguments),
                GenericParameterTypeSymbol sym => sym.DeclaresType,
                _ => throw ExhaustiveMatch.Failed(symbol)
            };
            return type;
        }
    }

    private DataType Evaluate(ITypeSyntax typeSyntax, ReferenceCapability capability)
    {
        switch (typeSyntax)
        {
            default:
                throw ExhaustiveMatch.Failed(typeSyntax);
            case ISimpleTypeNameSyntax syn:
                _ = EvaluateBareType(syn, capability);
                return syn.NamedType!;
            case IParameterizedTypeSyntax syn:
                _ = EvaluateBareType(syn, capability);
                return syn.NamedType!;
            case ICapabilityTypeSyntax _:
            case IOptionalTypeSyntax _:
                throw new NotImplementedException("Report error about incorrect type expression.");
        }
    }

    /// <summary>
    /// Evaluate a type that should not have any reference capability.
    /// </summary>
    /// <remarks>This is used for new expressions and base types. It assigns an `id` reference
    /// capability to the type.</remarks>
    public DataType EvaluateBareType(ITypeNameSyntax typeSyntax) => EvaluateBareType(typeSyntax, null);

    private DataType EvaluateBareType(
        ITypeNameSyntax typeSyntax,
        ReferenceCapability? capability)
    {
        return typeSyntax switch
        {
            ISimpleTypeNameSyntax syn => ResolveType(syn, FixedList<DataType>.Empty, CreateType),
            IParameterizedTypeSyntax syn
                => ResolveType(syn, Evaluate(syn.TypeArguments), CreateType),
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
                    return sym.DeclaresType;
                case ObjectTypeSymbol sym:
                    var bareType = sym.DeclaresType;
                    // If capability not provided, then this is for a constructor or something
                    // and reading the value doesn't matter, it exists to name the type.
                    capability ??= ReferenceCapability.Identity;
                    // Compatibility of the capability with the type is not checked here. That
                    // is done on the capability type syntax.
                    return bareType.With(capability, typeArguments);
                case GenericParameterTypeSymbol sym:
                    return sym.DeclaresType;
            }
        }
    }

    private FixedList<DataType> Evaluate(IEnumerable<ITypeSyntax> types)
        => types.Select(t => Evaluate(t, implicitRead: true)).ToFixedList();

    private DataType ResolveType(
        ITypeNameSyntax typeName,
        FixedList<DataType> typeArguments,
        Func<TypeSymbol, FixedList<DataType>, DataType> createType)
    {
        var symbols = typeName.LookupInContainingScope().ToFixedList();
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

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
    public ObjectType Evaluate(
        DeclaredObjectType objectType,
        IReferenceCapabilitySyntax capability,
        FixedList<DataType> typeArguments)
        => objectType.With(capability.Declared.ToReferenceCapability(), typeArguments);
}
