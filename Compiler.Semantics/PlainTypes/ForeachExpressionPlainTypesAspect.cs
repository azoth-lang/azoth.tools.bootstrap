using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

internal static partial class ForeachExpressionPlainTypesAspect
{
    public static partial ITypeDeclarationNode? ForeachExpression_ReferencedIterableDeclaration(IForeachExpressionNode node)
        => node.PackageNameScope().Lookup(node.InExpression?.PlainType ?? IPlainType.Unknown);

    public static partial IStandardMethodDeclarationNode? ForeachExpression_ReferencedIterateMethod(IForeachExpressionNode node)
        => node.ReferencedIterableDeclaration?.InclusiveInstanceMembersNamed("iterate").OfType<IStandardMethodDeclarationNode>()
               .Where(m => m.Arity == 0 && m.ReturnPlainType is NonVoidPlainType)
               .TrySingle();

    public static partial IMaybeNonVoidPlainType ForeachExpression_IteratorPlainType(IForeachExpressionNode node)
    {
        var iterableType = node.InExpression?.PlainType.ToNonVoid() ?? IPlainType.Unknown;
        var iterateMethod = node.ReferencedIterateMethod;
        var iteratorPlainType = iterateMethod is not null
            ? iterableType.ReplaceTypeParametersIn(iterateMethod.MethodGroupPlainType.Return).ToNonVoid()
            : iterableType;
        return iteratorPlainType;
    }

    public static partial ITypeDeclarationNode? ForeachExpression_ReferencedIteratorDeclaration(IForeachExpressionNode node)
        => node.PackageNameScope().Lookup(node.IteratorPlainType);

    public static partial IStandardMethodDeclarationNode? ForeachExpression_ReferencedNextMethod(IForeachExpressionNode node)
        => node.ReferencedIteratorDeclaration?.InclusiveInstanceMembersNamed("next").OfType<IStandardMethodDeclarationNode>()
               .Where(m => m.Arity == 0 && m.ReturnPlainType is OptionalPlainType)
               .TrySingle();

    public static partial IMaybeNonVoidPlainType ForeachExpression_IteratedPlainType(IForeachExpressionNode node)
    {
        var nextMethodReturnType = node.ReferencedNextMethod?.ReturnPlainType;
        if (nextMethodReturnType is OptionalPlainType { Referent: var iteratedType })
            return node.IteratorPlainType.ReplaceTypeParametersIn(iteratedType).ToNonLiteral().ToNonVoid();
        return IPlainType.Unknown;
    }
}
