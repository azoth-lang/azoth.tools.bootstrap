namespace Azoth.Tools.Bootstrap.Compiler.Emit.C
{
    public interface IConverter<in T>
    {
        string Convert(T value);
    }
}
