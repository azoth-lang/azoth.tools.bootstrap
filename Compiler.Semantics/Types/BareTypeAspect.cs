using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class BareTypeAspect
{
    public static partial BareType? IdentifierTypeName_NamedBareType(IIdentifierTypeNameNode node)
        => BuildBareType(node.ReferencedSymbol, []);

    // TODO this should avoid using symbols and use referenced declarations instead
    private static BareType? BuildBareType(TypeSymbol? symbol, IFixedList<IMaybeType> typeArguments)
    {
        // Empty and generic parameter types don't have a declared or bare type. Note: the number
        // of arguments will match the type because name binding will only pick a matching type.
        var declaredType = symbol?.GetDeclaredType();
        return declaredType is not null ? BuildBareType(declaredType, typeArguments) : null;
    }

    private static BareType? BuildBareType(DeclaredType type, IFixedList<IMaybeType> typeArguments)
        // The number of arguments will match the type because name binding will only pick a matching type
        => type.With(typeArguments);

    public static partial BareType? GenericTypeName_NamedBareType(IGenericTypeNameNode node)
        => BuildBareType(node.ReferencedSymbol, node.TypeArguments.Select(t => t.NamedType).ToFixedList());

    public static partial BareType? SpecialTypeName_NamedBareType(ISpecialTypeNameNode node)
        => BuildBareType(node.ReferencedSymbol, []);

    public static partial BareType? TypeNameExpression_NamedBareType(ITypeNameExpressionNode node)
    {
        var referencedSymbol = node.ReferencedDeclaration.Symbol;
        var declaredType = referencedSymbol.GetDeclaredType();
        if (declaredType is null)
            return null;
        var typeArguments = node.TypeArguments.Select(a => a.NamedType).ToFixedList();
        if (typeArguments.Count != node.TypeArguments.Count)
            return null;
        return declaredType.With(typeArguments);
    }
}
