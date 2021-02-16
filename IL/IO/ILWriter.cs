using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Azoth.Tools.Bootstrap.Framework;
using Be.IO;
using Force.Crc32;

namespace Azoth.Tools.Bootstrap.IL.IO
{
    public class ILWriter
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static",
            Justification = "OO")]
        public void Write(PackageIL package, Stream outputStream)
        {
            using var writer = new BeBinaryWriter(outputStream, ILFile.StringEncoding, true);

            WriteFileSignature(writer, package.EntryPointFunctionID != null);
            WriteFormatVersion(writer);
            var indexOffset = WriteIndexPlaceholder(writer, package);
            var packageMetadataOffset = WritePackage(writer, package);
            var functionsOffset = WriteFunctions(writer, package.Functions);
            var eofOffset = writer.BaseStream.Position;
            WriteUpdatedIndex(writer, package,
                indexOffset,
                packageMetadataOffset,
                functionsOffset,
                eofOffset);
        }

        private static void WriteFileSignature(BinaryWriter writer, bool isExecutable)
        {
            var signature = isExecutable ? ILFile.ExecutableSignatureBytes : ILFile.PackageSignatureBytes;
            writer.Write(signature.Span);
        }

        private static void WriteFormatVersion(BinaryWriter writer)
        {
            // v0.1
            writer.Write((ushort)0);
            writer.Write((ushort)1);
        }

        private static long WriteIndexPlaceholder(BinaryWriter writer, PackageIL package)
        {
            var offset = writer.BaseStream.Position;

            WriteIndexSegment(writer, SegmentNumber.Package, 0);
            if (package.Functions.Count > 0)
                WriteIndexSegment(writer, SegmentNumber.Functions, 0);

            // Terminate the index
            WriteIndexSegment(writer, SegmentNumber.EOF, 0);
            // Checksum Placeholder
            writer.Write((uint)0);

            return offset;
        }

        private static void WriteUpdatedIndex(
            BinaryWriter writer,
            PackageIL package,
            long indexOffset,
            long packageMetadataOffset,
            long functionsOffset,
            long eofOffset)
        {
            // Seek to the start of the index
            writer.BaseStream.Position = indexOffset;

            WriteIndexSegment(writer, SegmentNumber.Package, packageMetadataOffset);
            if (package.Functions.Count > 0)
                WriteIndexSegment(writer, SegmentNumber.Functions, functionsOffset);

            // Terminate the index
            WriteIndexSegment(writer, SegmentNumber.EOF, eofOffset);

            // Read back the header to compute the CRC32C
            var header = new byte[writer.BaseStream.Position];
            writer.BaseStream.Position = 0;
            writer.BaseStream.Read(header.AsSpan());
            // Since we just read the header, we are now back at the checksum offset
            writer.Write(Crc32CAlgorithm.Compute(header));
        }

        private static void WriteIndexSegment(BinaryWriter writer, SegmentNumber segment, long offset)
        {
            writer.Write((ushort)segment);
            writer.Write(offset);
        }

        private static long WritePackage(BinaryWriter writer, PackageIL package)
        {
            var offset = writer.BaseStream.Position;
            // Entry Function ID
            writer.Write((int?)package.EntryPointFunctionID  ?? -1);

            return offset;
        }

        private static long WriteFunctions(BinaryWriter writer, FixedList<FunctionIL> functions)
        {
            var offset = writer.BaseStream.Position;

            // Function count
            writer.Write((uint)functions.Count);

            foreach (var (function, i) in functions.Enumerate())
            {

                // Control flow graph id
                writer.Write(i);
            }
            return offset;
        }
    }
}
