namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

public interface IAttributeFunction<TNode, T>
{
    T Compute(TNode node, IInheritanceContext ctx);
}
