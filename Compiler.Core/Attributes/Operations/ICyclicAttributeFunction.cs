namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

public interface ICyclicAttributeFunction<in TNode, T>
{
    T Compute(TNode node, T current);
}
