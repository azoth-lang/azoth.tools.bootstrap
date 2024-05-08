using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class BareTypeAttribute
{
    public static BareType? IdentifierTypeName(IIdentifierTypeNameNode node)
        => BuildBareType(node.ReferencedSymbol, FixedList.Empty<DataType>());

    private static BareType? BuildBareType(TypeSymbol? symbol, IFixedList<DataType> typeArguments)
        => symbol switch
        {
            null => null,
            EmptyTypeSymbol _ => null, // Empty types don't have a declared or bare type
            PrimitiveTypeSymbol sym => BuildBareType(sym.DeclaresType, typeArguments),
            UserTypeSymbol sym => BuildBareType(sym.DeclaresType.AsDeclaredType(), typeArguments),
            GenericParameterTypeSymbol _ => null, // Generic parameter types don't have a declared or bare type
            _ => throw ExhaustiveMatch.Failed(symbol)
        };

    private static BareType? BuildBareType(DeclaredType type, IFixedList<DataType> typeArguments)
        => type.GenericParameters.Count == typeArguments.Count ? type.With(typeArguments) : null;

    public static BareType? GenericTypeName(IGenericTypeNameNode node)
        => BuildBareType(node.ReferencedSymbol, node.TypeArguments.Select(t => t.Type).ToFixedList());

    public static BareType SpecialTypeName(ISpecialTypeNameNode node)
        => BuildBareType(node.ReferencedSymbol, FixedList.Empty<DataType>())
            ?? throw new UnreachableException();
}
