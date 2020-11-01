using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage;

namespace Azoth.Tools.Bootstrap.Compiler.Emit
{
    public abstract class Emitter
    {
        public abstract void Emit(PackageIL package);
        public abstract string GetEmittedCode();
    }
}
