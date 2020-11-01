namespace Azoth.Tools.Bootstrap.Compiler.Emit.C
{
    public interface IEmitter<in T>
    {
        void Emit(T value, Code code);
    }
}
