using System.Diagnostics.CodeAnalysis;
using System.IO;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.IL.IO
{
    public class ILReader
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static",
            Justification = "OO")]
        public PackageIL Read(Stream inputStream)
        {
            using var reader = new BinaryReader(inputStream, ILFile.StringEncoding, leaveOpen: true);
            var isExecutable = ReadSignature(reader);

            return new PackageIL(
                FixedList<PackageReferenceIL>.Empty,
                FixedList<string>.Empty,
                FixedList<string>.Empty,
                FixedList<ClassIL>.Empty,
                FixedList<FunctionIL>.Empty,
                FixedList<DataType>.Empty,
                null);
        }

        private static bool ReadSignature(BinaryReader reader)
        {
            var signature = new string(reader.ReadChars(ILFile.SignatureLength));
            return signature switch
            {
                ILFile.PackageSignature => false,
                ILFile.ExecutableSignature => true,
                _ => throw new FileFormatException()
            };
        }
    }
}
