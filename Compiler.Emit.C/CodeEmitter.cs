using Azoth.Tools.Bootstrap.Compiler.Emit.C.Properties;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage;

namespace Azoth.Tools.Bootstrap.Compiler.Emit.C
{
    public class CodeEmitter : Emitter
    {
        public static string RuntimeLibraryCode => Resources.RuntimeLibraryCode;
        public const string RuntimeLibraryCodeFileName = "RuntimeLibrary.c";
        public static string RuntimeLibraryHeader => Resources.RuntimeLibraryHeader;
        public const string RuntimeLibraryHeaderFileName = "RuntimeLibrary.h";

        private readonly PackageEmitter packageEmitter;
        private readonly Code code = new Code();

        public CodeEmitter()
        {
            var nameMangler = new NameMangler();
            var typeConverter = new TypeConverter(nameMangler);
            var parameterConverter = new ParameterConverter(nameMangler, typeConverter);
            var controlFlowEmitter = new ControlFlowEmitter(nameMangler, typeConverter);
            var declarationEmitter = new DeclarationEmitter(nameMangler, parameterConverter, typeConverter, controlFlowEmitter);
            packageEmitter = new PackageEmitter(nameMangler, declarationEmitter);
            packageEmitter.EmitPreamble(code);
        }

        public override void Emit(PackageIL package)
        {
            packageEmitter.Emit(package, code);
        }

        public override string GetEmittedCode()
        {
            packageEmitter.EmitPostamble(code);
            return code.ToString();
        }
    }
}
