using System;
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
        switch (typeSyntax)
        {
            default:
                throw ExhaustiveMatch.Failed(typeSyntax);
            case null:
                return null;
            case ITypeNameSyntax typeName:
            {
                var symbolPromises = typeName.LookupInContainingScope().ToFixedList();
                switch (symbolPromises.Count)
                {
                    case 0:
                        diagnostics.Add(NameBindingError.CouldNotBindName(file, typeName.Span));
                        typeName.ReferencedSymbol.Fulfill(null);
                        typeName.NamedType = DataType.Unknown;
                        break;
                    case 1:
                        var symbol = symbolPromises.Single().Result;
                        typeName.ReferencedSymbol.Fulfill(symbol);
                        var type = symbol switch
                        {
                            PrimitiveTypeSymbol sym => sym.DeclaresType,
                            ObjectTypeSymbol sym => implicitRead || sym.DeclaresType.IsConst
                                ? sym.DeclaresType.WithRead()
                                : sym.DeclaresType.WithMutate(),
                            _ => throw ExhaustiveMatch.Failed(symbol)
                        };
                        typeName.NamedType = type;
                        break;
                    default:
                        diagnostics.Add(NameBindingError.AmbiguousName(file, typeName.Span));
                        typeName.ReferencedSymbol.Fulfill(null);
                        typeName.NamedType = DataType.Unknown;
                        break;
                }
                break;
            }
            case ICapabilityTypeSyntax referenceCapability:
            {
                var capability = referenceCapability.Capability.Declared.ToReferenceCapability();
                var type = Evaluate(referenceCapability.ReferentType, capability);
                if (capability.AllowsWrite && type is ObjectType { IsConst: true } objectType)
                    diagnostics.Add(TypeError.CannotApplyCapabilityToConstantType(file, referenceCapability, capability,
                        objectType.BareType));
                return referenceCapability.NamedType = type;
            }
            case IOptionalTypeSyntax optionalType:
            {
                var referent = Evaluate(optionalType.Referent, implicitRead);
                return optionalType.NamedType = new OptionalType(referent);
            }
        }

        return typeSyntax.NamedType ?? throw new InvalidOperationException();
    }

    private DataType Evaluate(ITypeSyntax typeSyntax, ReferenceCapability capability)
    {
        switch (typeSyntax)
        {
            default:
                throw ExhaustiveMatch.Failed(typeSyntax);
            case ITypeNameSyntax typeName:
                var bareType = EvaluateBareType(typeName, capability);
                return bareType?.With(capability) ?? (DataType)DataType.Unknown;
            case ICapabilityTypeSyntax _:
            case IOptionalTypeSyntax _:
                throw new NotImplementedException("Report error about incorrect type expression.");
        }
    }

    public BareReferenceType? EvaluateBareType(ITypeNameSyntax typeName, ReferenceCapability? capability = null)
    {
        var symbolPromises = typeName.LookupInContainingScope().ToFixedList();
        switch (symbolPromises.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindName(file, typeName.Span));
                typeName.ReferencedSymbol.Fulfill(null);
                typeName.NamedType = DataType.Unknown;
                return null;
            case 1:
                var symbol = symbolPromises.Single().Result;
                typeName.ReferencedSymbol.Fulfill(symbol);
                switch (symbol)
                {
                    default:
                        throw ExhaustiveMatch.Failed(symbol);
                    case PrimitiveTypeSymbol sym:
                        typeName.NamedType = sym.DeclaresType;
                        return null;
                    case ObjectTypeSymbol sym:
                        // TODO this is odd because without
                        var bareType = sym.DeclaresType;
                        // If capability not provided, then this is for a constructor or something
                        // and reading the value doesn't matter, it exists to name the type.
                        capability ??= ReferenceCapability.Identity;
                        // Compatibility of the capability with the type is not checked here. That
                        // is done on the capability
                        typeName.NamedType = bareType.With(capability);
                        return bareType;

                }
            default:
                diagnostics.Add(NameBindingError.AmbiguousName(file, typeName.Span));
                typeName.ReferencedSymbol.Fulfill(null);
                typeName.NamedType = DataType.Unknown;
                return null;
        }
    }

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
    public ObjectType Evaluate(BareObjectType referenceType, IReferenceCapabilitySyntax capability)
        => referenceType.With(capability.Declared.ToReferenceCapability());
}
