using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class BareTypeAspect
{
    #region Type Names
    public static partial BareType? BuiltInTypeName_NamedBareType(IBuiltInTypeNameNode node)
        => NamedBareType(containingType: null, node, []);

    public static partial BareType? IdentifierTypeName_NamedBareType(IIdentifierTypeNameNode node)
        => NamedBareType(containingType: null, node, []);

    public static partial BareType? GenericTypeName_NamedBareType(IGenericTypeNameNode node)
        => NamedBareType(containingType: null, node, node.GenericArguments);

    public static partial BareType? QualifiedTypeName_NamedBareType(IQualifiedTypeNameNode node)
        => NamedBareType(node.Context.NamedBareType, node, node.GenericArguments);

    private static BareType? NamedBareType(BareType? containingType, ITypeNameNode node, IFixedList<ITypeNode> genericArguments)
    {
        var referencedDeclaration = node.ReferencedDeclaration;
        if (referencedDeclaration is null) return null;
        var args = genericArguments.IsEmpty ? FixedList.Empty<IMaybeType>()
            : genericArguments.Select(t => t.NamedType).ToFixedList();
        // The number of arguments will match the type because name binding will only pick a matching type
        return referencedDeclaration.TypeConstructor.TryConstructBareType(containingType, args);
    }
    #endregion
}
