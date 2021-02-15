using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Azoth.Tools.Bootstrap.IL.IO
{
    public class ILWriter
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static",
            Justification = "OO")]
        public void Write(PackageIL package, Stream outputStream)
        {
            using var writer = new BinaryWriter(outputStream, ILFile.StringEncoding, true);

            EmitFileSignature(writer, package.EntryPoint != null);
        }

        public static void EmitFileSignature(BinaryWriter writer, bool isExecutable)
        {
            var signature = isExecutable ? ILFile.ExecutableSignatureBytes : ILFile.PackageSignatureBytes;
            writer.Write(signature.Span);
        }
    }
}
