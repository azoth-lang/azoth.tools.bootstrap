using System;
using System.Linq;
using System.Text;

namespace Azoth.Tools.Bootstrap.IL.IO
{
    public static class ILFile
    {
        public const int SignatureLength = 16;
        public const string PackageSignature = "\x0089Azoth IL-P\r\n\x1A\r\0";
        public static readonly ReadOnlyMemory<byte> PackageSignatureBytes
            = PackageSignature.Select(c => (byte)c).ToArray();
        public const string ExecutableSignature = "\x0089Azoth IL-X\r\n\x1A\r\0";
        public static readonly ReadOnlyMemory<byte> ExecutableSignatureBytes
            = ExecutableSignature.Select(c => (byte)c).ToArray();
        public static readonly Encoding StringEncoding = new UTF8Encoding(false, true);
    }
}
