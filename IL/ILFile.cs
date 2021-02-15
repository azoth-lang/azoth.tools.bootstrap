using System;
using System.Text;

namespace Azoth.Tools.Bootstrap.IL
{
    public static class ILFile
    {
        public const int SignatureLength = 8;
        public const string PackageSignature = "AzothILP";
        public static readonly ReadOnlyMemory<byte> PackageSignatureBytes
            = Encoding.ASCII.GetBytes(PackageSignature);
        public const string ExecutableSignature = "AzothILX";
        public static readonly ReadOnlyMemory<byte> ExecutableSignatureBytes
            = Encoding.ASCII.GetBytes(ExecutableSignature);
        public static readonly Encoding StringEncoding = new UTF8Encoding(false, true);
    }
}
