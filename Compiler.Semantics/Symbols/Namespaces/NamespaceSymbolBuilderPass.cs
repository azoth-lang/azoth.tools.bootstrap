namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Namespaces;

// TODO take a context?
// TODO return a context?

public sealed class NamespaceSymbolBuilderPass
{
    // ShouldVisitX
    // Rewrite(TParam, OldNode) -> NewNode
    // Enter(TParam, OldNode) -> TParam (Only supported on same lang?)
    // Rewrite(OldNode, NewChild1, NewChild2, ...) -> NewNode

    // What if things took Lazy thereby allowing for on demand eval?

    // NewPromise uses async/await to allow for on demand eval
    // If each child and child list is a NewPromise


    // Can have "passes" that just document how to add attributes to nodes
    // Have rewrites be separate passes

    // What actually pushes the eval?

    // Example: Simple expression typing with constant folding

    //public async Promise<L2.BinaryOp> Rewrite(L1.BinaryOp node)
    //{
    //    var left = Rewrite(node.Left);
    //    var right = Rewrite(node.Right);
    //    var type = CombineTypes((await left).Type, (await right).Type);
    //    return new L2.BinaryOp(node.Op, left, right, type);
    //}

    //public async Promise<L3.Expression> Rewrite(L2.BinaryOp node)
    //{

    //}

    //// Compose
    //public async Promise<L3.Expression> Rewrite(L1.Expression exp1)
    //{
    //    var exp2 = pass1.Rewrite(exp);
    //}

    //public async Promise<L3.BinaryOp> Rewrite(L1.BinaryOp exp1)
    //{

    //}

    //public async Promise<L2.BinaryOp> Rewrite(Promise<L1.BinaryOp> old)
    //{
    //    var node = await old;
    //    var left = Rewrite(node.Left);
    //    var right = Rewrite(node.Right);
    //    var leftType = await left.Select(l => l.Type);
    //    var rightType = await right.Select(r => r.Type);
    //    var type = ComputeType(leftType, rightType);
    //    return new L2.BinaryOp(node.Op, left, right, type);
    //}

    //public async Promise<L2.BinaryOp> Rewrite(Promise<L1.BinaryOp> old)
    //{
    //    var node = await old;
    //    var left = Rewrite(node.Left);
    //    var right = Rewrite(node.Right);
    //    var leftType = Final<IHasType>(left).Type;
    //    var rightType = await right.Select(r => r.Type);
    //    var type = ComputeType(leftType, rightType);
    //    return new L2.BinaryOp(node.Op, left, right, type);
    //}
    //// -------------------------------

    //public Shadow<L2.BinaryOp> Rewrite(Phantom<L1.BinaryOp> old)
    //{
    //    var left = Rewrite(old.Select(o => Left)); // Shadow<L2.Expression>
    //    var right = Rewrite(old.Select(o => Right)); // Shadow<L2.Expression>
    //    var leftType = left.Select(l => l.Type); // Promise<L2.Type>
    //    var rightType = right.Select(r => r.Type); // Promise<L2.Type>
    //    var type = ComputeType(leftType, rightType); // Promise<L2.Type>
    //    return Rewrite(old, left, right, type);
    //}

    //// -------------------------------

    //public Phantom<L2.BinaryOp> Rewrite(Phantom<L1.BinaryOp> old)
    //{
    //    var left = old.Rewrite(o => Left); // Phantom<L2.Expression>
    //    var right = old.Rewrite(o => Right); // Phantom<L2.Expression>
    //    var leftType = left.Select(l => l.Type); // L2.Type
    //    var rightType = right.Select(r => r.Type); // L2.Type
    //    var type = ComputeType(leftType, rightType); // L2.Type
    //    return Rewrite(old, left, right, type);
    //}

    //public TResult Rewrite<TLeft, TRight, TResult>(L1.BinaryOp.Data nodeData, TLeft left, TRight right)
    //    where TLeft : TResult, IHasType
    //    where TRight : IHasType
    //{
    //    var type = ComputeType(left.Type, right.Type);
    //    return Rewrite(old, left, right, type);
    //}
}
