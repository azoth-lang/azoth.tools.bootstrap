using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class BareTypeAspect
{
    public static partial BareType? IdentifierTypeName_NamedBareType(IIdentifierTypeNameNode node)
        => BuildBareType(node.ReferencedDeclaration?.TypeConstructor, []);

    public static partial BareType? GenericTypeName_NamedBareType(IGenericTypeNameNode node)
        => BuildBareType(node.ReferencedDeclaration?.TypeConstructor, node.TypeArguments.Select(t => t.NamedType).ToFixedList());

    public static partial BareType? BuiltInTypeName_NamedBareType(IBuiltInTypeNameNode node)
        => BuildBareType(node.ReferencedDeclaration?.TypeConstructor, []);

    private static BareType? BuildBareType(ITypeConstructor? typeConstructor, IFixedList<IMaybeType> typeArguments)
        // The number of arguments will match the type because name binding will only pick a matching type
        => typeConstructor?.TryConstruct(containingType: null, typeArguments);

    public static partial BareType? TypeNameExpression_NamedBareType(ITypeNameExpressionNode node)
    {
        var typeFactory = node.ReferencedDeclaration.TypeConstructor;
        var typeArguments = node.TypeArguments.Select(a => a.NamedType).ToFixedList();
        if (typeArguments.Count != node.TypeArguments.Count)
            return null;
        return typeFactory.TryConstruct(containingType: null, typeArguments);
    }
}
