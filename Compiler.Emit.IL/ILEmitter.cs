using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Azoth.Tools.Bootstrap.IL;
using PackageIL = Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.PackageIL;

namespace Azoth.Tools.Bootstrap.Compiler.Emit.IL
{
    public class ILEmitter
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static",
            Justification = "OO")]
        public void Emit(PackageIL package, Stream outputStream)
        {
            using var writer = new BinaryWriter(outputStream, ILFile.StringEncoding, true);

            EmitFileSignature(writer, package.EntryPoint != null);
        }

        public static void EmitFileSignature(BinaryWriter writer, bool isExecutable)
        {
            var signature = isExecutable ? ILFile.ExecutableSignatureBytes
                                         : ILFile.PackageSignatureBytes;
            writer.Write(signature.Span);
        }
    }
}
