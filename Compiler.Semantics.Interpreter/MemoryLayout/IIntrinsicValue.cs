using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

public interface IIntrinsicValue
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sealed T UnsafeAs<T>()
        where T : class, IIntrinsicValue
        => Unsafe.As<T>(this);
}
