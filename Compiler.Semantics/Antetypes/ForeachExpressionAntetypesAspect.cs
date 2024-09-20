using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static partial class ForeachExpressionAntetypesAspect
{
    public static partial ITypeDeclarationNode? ForeachExpression_ReferencedIterableDeclaration(IForeachExpressionNode node)
        => node.PackageNameScope().Lookup(node.InExpression?.Antetype ?? IAntetype.Unknown);

    public static partial IStandardMethodDeclarationNode? ForeachExpression_ReferencedIterateMethod(IForeachExpressionNode node)
        => node.ReferencedIterableDeclaration?.InclusiveInstanceMembersNamed("iterate").OfType<IStandardMethodDeclarationNode>()
               .Where(m => m.Arity == 0 && m.MethodGroupType.Return.ToAntetype() is INonVoidAntetype)
               .TrySingle();

    public static partial IMaybeExpressionAntetype ForeachExpression_IteratorAntetype(IForeachExpressionNode node)
    {
        var iterableType = node.InExpression?.Antetype ?? IAntetype.Unknown;
        var iterateMethod = node.ReferencedIterateMethod;
        var iteratorAntetype = iterateMethod is not null
            ? iterableType.ReplaceTypeParametersIn(iterateMethod.MethodGroupType.Return.ToAntetype())
            : iterableType;
        return iteratorAntetype;
    }

    public static partial ITypeDeclarationNode? ForeachExpression_ReferencedIteratorDeclaration(IForeachExpressionNode node)
        => node.PackageNameScope().Lookup(node.IteratorAntetype);

    public static partial IStandardMethodDeclarationNode? ForeachExpression_ReferencedNextMethod(IForeachExpressionNode node)
        => node.ReferencedIteratorDeclaration?.InclusiveInstanceMembersNamed("next").OfType<IStandardMethodDeclarationNode>()
               .Where(m => m.Arity == 0 && m.MethodGroupType.Return.ToAntetype() is OptionalAntetype)
               .TrySingle();

    public static partial IMaybeAntetype ForeachExpression_IteratedAntetype(IForeachExpressionNode node)
    {
        var nextMethodReturnType = node.ReferencedNextMethod?.MethodGroupType.Return.ToAntetype();
        if (nextMethodReturnType is OptionalAntetype { Referent: var iteratedType })
            return node.IteratorAntetype.ReplaceTypeParametersIn(iteratedType).ToNonConstValueType();
        return IAntetype.Unknown;
    }
}
