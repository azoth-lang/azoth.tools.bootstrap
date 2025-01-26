using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class BareTypeAspect
{
    public static partial BareType? IdentifierTypeName_NamedBareType(IIdentifierTypeNameNode node)
        => BuildBareType(node, []);

    public static partial BareType? GenericTypeName_NamedBareType(IGenericTypeNameNode node)
        => BuildBareType(node, node.GenericArguments.Select(t => t.NamedType).ToFixedList());

    public static partial BareType? BuiltInTypeName_NamedBareType(IBuiltInTypeNameNode node)
        => BuildBareType(node, []);

    private static BareType? BuildBareType(ITypeNameNode node, IFixedList<IMaybeType> typeArguments)
    {
        var typeConstructor = node.ReferencedDeclaration?.TypeConstructor;
        // The number of arguments will match the type because name binding will only pick a matching type
        return typeConstructor?.TryConstruct(containingType: null, typeArguments);
    }
}
