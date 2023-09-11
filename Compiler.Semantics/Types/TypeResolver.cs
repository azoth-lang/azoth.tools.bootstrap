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
using Void = Azoth.Tools.Bootstrap.Framework.Void;

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
            case ISimpleTypeNameSyntax typeName:
            {
                _ = ResolveType(typeName, AssignTypeName);
                break;
            }
            case IParameterizedTypeSyntax syn:
            {
                throw new NotImplementedException("Resolution of parameterized type names not implemented");
            }
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

        return typeSyntax.NamedType ?? throw new InvalidOperationException();

        Void AssignTypeName(ITypeNameSyntax typeName, TypeSymbol symbol)
        {
            var type = symbol switch
            {
                PrimitiveTypeSymbol sym => sym.DeclaresType,
                ObjectTypeSymbol sym => implicitRead || sym.DeclaresType.IsConst
                    ? sym.DeclaresType.WithRead()
                    : sym.DeclaresType.WithMutate(),
                _ => throw ExhaustiveMatch.Failed(symbol)
            };
            typeName.NamedType = type;
            return default;
        }
    }

    private DataType Evaluate(ITypeSyntax typeSyntax, ReferenceCapability capability)
    {
        switch (typeSyntax)
        {
            default:
                throw ExhaustiveMatch.Failed(typeSyntax);
            case ISimpleTypeNameSyntax typeName:
                var bareType = EvaluateBareType(typeName, capability);
                return bareType?.With(capability) ?? (DataType)DataType.Unknown;
            case IParameterizedTypeSyntax syn:
                throw new NotImplementedException("Parameterized types with reference capabilities not implemented");
            case ICapabilityTypeSyntax _:
            case IOptionalTypeSyntax _:
                throw new NotImplementedException("Report error about incorrect type expression.");
        }
    }

    public DeclaredReferenceType? EvaluateBareType(ITypeNameSyntax typeName, ReferenceCapability? capability = null)
    {
        return ResolveType(typeName, AssignNamedType);

        DeclaredReferenceType? AssignNamedType(
            ITypeNameSyntax _,
            TypeSymbol symbol)
        {
            switch (symbol)
            {
                default:
                    throw ExhaustiveMatch.Failed(symbol);
                case PrimitiveTypeSymbol sym:
                    typeName.NamedType = sym.DeclaresType;
                    return null;
                case ObjectTypeSymbol sym:
                    var bareType = sym.DeclaresType;
                    // If capability not provided, then this is for a constructor or something
                    // and reading the value doesn't matter, it exists to name the type.
                    capability ??= ReferenceCapability.Identity;
                    // Compatibility of the capability with the type is not checked here. That
                    // is done on the capability type syntax.
                    typeName.NamedType = bareType.With(capability);
                    return bareType;
            }
        }
    }

    private TResult? ResolveType<TResult>(
        ITypeNameSyntax typeName,
        Func<ITypeNameSyntax, TypeSymbol, TResult?> assignNamedType)
        where TResult : notnull
    {
        var symbols = typeName.LookupInContainingScope().ToFixedList();
        switch (symbols.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindName(file, typeName.Span));
                typeName.ReferencedSymbol.Fulfill(null);
                typeName.NamedType = DataType.Unknown;
                return default;
            case 1:
                var symbol = symbols.Single();
                typeName.ReferencedSymbol.Fulfill(symbol);
                return assignNamedType(typeName, symbol);
            default:
                diagnostics.Add(NameBindingError.AmbiguousName(file, typeName.Span));
                typeName.ReferencedSymbol.Fulfill(null);
                typeName.NamedType = DataType.Unknown;
                return default;
        }
    }

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
    public ObjectType Evaluate(DeclaredObjectType referenceType, IReferenceCapabilitySyntax capability)
        => referenceType.With(capability.Declared.ToReferenceCapability());
}
