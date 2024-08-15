namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

public interface IAttributeFunction<in TNode, out T>
{
    T Compute(TNode node, IInheritanceContext ctx);
}
