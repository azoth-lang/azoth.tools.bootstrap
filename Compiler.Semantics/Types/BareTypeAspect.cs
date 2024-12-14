using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class BareTypeAspect
{
    // TODO should this be removed? Does it duplicate SelfPlainType?
    // No, but a bare self type should not be a plain type, it needs to have arguments for the containing type
    public static partial SelfPlainType TypeDefinition_BareSelfType(ITypeDefinitionNode node)
        => new(node.TypeConstructor);

    public static partial BareType? IdentifierTypeName_NamedBareType(IIdentifierTypeNameNode node)
        => BuildBareType(node.ReferencedDeclaration?.TypeConstructor, []);

    public static partial BareType? GenericTypeName_NamedBareType(IGenericTypeNameNode node)
        => BuildBareType(node.ReferencedDeclaration?.TypeConstructor, node.TypeArguments.Select(t => t.NamedType).ToFixedList());

    public static partial BareType? SpecialTypeName_NamedBareType(ISpecialTypeNameNode node)
        => BuildBareType(node.ReferencedDeclaration?.TypeConstructor, []);

    private static BareType? BuildBareType(TypeConstructor? typeConstructor, IFixedList<IMaybeType> typeArguments)
        // The number of arguments will match the type because name binding will only pick a matching type
        => typeConstructor?.TryConstruct(typeArguments);

    public static partial BareType? TypeNameExpression_NamedBareType(ITypeNameExpressionNode node)
    {
        var typeConstructor = node.ReferencedDeclaration.TypeConstructor;
        if (typeConstructor is null)
            return null;
        var typeArguments = node.TypeArguments.Select(a => a.NamedType).ToFixedList();
        if (typeArguments.Count != node.TypeArguments.Count)
            return null;
        return typeConstructor.TryConstruct(typeArguments);
    }
}
