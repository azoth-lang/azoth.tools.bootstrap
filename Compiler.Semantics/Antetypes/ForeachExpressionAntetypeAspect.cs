using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static class ForeachExpressionAntetypeAspect
{
    public static ITypeDeclarationNode? ForeachExpression_ReferencedIterableDeclaration(IForeachExpressionNode node)
        => node.InheritedPackageNameScope().Lookup(node.FinalInExpression.Antetype);

    public static IStandardMethodDeclarationNode? ForeachExpression_ReferencedIterateMethod(IForeachExpressionNode node)
        => node.ReferencedIterableDeclaration?.InclusiveInstanceMembersNamed("iterate").OfType<IStandardMethodDeclarationNode>()
               .Where(m => m.Arity == 0 && m.MethodGroupType.Return.Type.ToAntetype() is INonVoidAntetype)
               .TrySingle();

    public static IMaybeExpressionAntetype ForeachExpression_IteratorAntetype(IForeachExpressionNode node)
    {
        var iterableType = node.FinalInExpression.Antetype;
        var iterateMethod = node.ReferencedIterateMethod;
        var iteratorAntetype = iterateMethod is not null
            ? iterableType.ReplaceTypeParametersIn(iterateMethod.MethodGroupType.Return.Type.ToAntetype())
            : iterableType;
        return iteratorAntetype;
    }

    public static ITypeDeclarationNode? ForeachExpression_ReferencedIteratorDeclaration(IForeachExpressionNode node)
        => node.InheritedPackageNameScope().Lookup(node.IteratorAntetype);

    public static IStandardMethodDeclarationNode? ForeachExpression_ReferencedNextMethod(IForeachExpressionNode node)
        => node.ReferencedIteratorDeclaration?.InclusiveInstanceMembersNamed("next").OfType<IStandardMethodDeclarationNode>()
               .Where(m => m.Arity == 0 && m.MethodGroupType.Return.Type.ToAntetype() is OptionalAntetype)
               .TrySingle();

    public static IMaybeAntetype ForeachExpression_IteratedAntetype(IForeachExpressionNode node)
    {
        var nextMethodReturnType = node.ReferencedNextMethod?.MethodGroupType.Return.Type.ToAntetype();
        if (nextMethodReturnType is OptionalAntetype { Referent: var iteratedType })
            return node.IteratorAntetype.ReplaceTypeParametersIn(iteratedType).ToNonConstValueType();
        return IAntetype.Unknown;
    }
}
