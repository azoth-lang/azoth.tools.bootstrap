using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Azoth.Tools.Bootstrap.IL.IO
{
    public class ILReader
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static",
            Justification = "OO")]
        public PackageIL Read(Stream inputStream)
        {
            using var reader = new BinaryReader(inputStream, ILFile.StringEncoding);
            var isExecutable = ReadSignature(reader);

            throw new NotImplementedException();
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
