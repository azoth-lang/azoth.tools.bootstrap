using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public interface IRewritableChildList<out TChild> : IFixedList<TChild>
{
    IFixedList<TChild> Current { get; }
}


public interface IRewritableChildList<out TChild, out TFinal> : IRewritableChildList<TChild>
{
    IFixedList<TFinal?> Intermediate { get; }
}
