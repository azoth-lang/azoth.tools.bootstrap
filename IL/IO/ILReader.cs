using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using Be.IO;
using Force.Crc32;

namespace Azoth.Tools.Bootstrap.IL.IO
{
    public class ILReader
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static",
            Justification = "OO")]
        public PackageIL Read(Stream inputStream)
        {
            using var reader = new BeBinaryReader(inputStream, ILFile.StringEncoding, leaveOpen: true);
            var isExecutable = ReadSignature(reader);
            ReadFormatVersion(reader);
            var index = ReadIndex(reader);

            ReadPackageMetadata(reader, index[SegmentNumber.Package]);

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
            var chars = reader.ReadBytes(ILFile.SignatureLength).Select(b => (char)b).ToArray();
            var signature = new string(chars);
            return signature switch
            {
                ILFile.PackageSignature => false,
                ILFile.ExecutableSignature => true,
                _ => throw new FileFormatException()
            };
        }

        private static void ReadFormatVersion(BinaryReader reader)
        {
            var major = reader.ReadUInt16();
            var minor = reader.ReadUInt16();
            if (major != 0 || minor != 1)
                throw new FileFormatException($"Unsupported file format version v{major}.{minor}");
        }

        private static Dictionary<SegmentNumber, long> ReadIndex(BinaryReader reader)
        {
            var index = new Dictionary<SegmentNumber, long>();

            SegmentNumber segment;
            do
            {
                segment = (SegmentNumber)reader.ReadUInt16();
                var offset = reader.ReadInt64();
                index.Add(segment, offset);
            } while (segment != SegmentNumber.EOF);

            if (index[SegmentNumber.EOF] != reader.BaseStream.Length)
                throw new FileFormatException("Incorrect file length in header");
            VerifyHeaderChecksum(reader);

            // Check that all offsets are past the end of the header
            var startOfData = reader.BaseStream.Position;
            if (index.Values.Any(v => v < startOfData))
                throw new FileFormatException("Segment offset is in header");

            return index;
        }

        private static void VerifyHeaderChecksum(BinaryReader reader)
        {
            var header = new byte[reader.BaseStream.Position];
            reader.BaseStream.Position = 0;
            reader.BaseStream.Read(header.AsSpan());
            // Since we just read the header, we are now back at the checksum offset
            var checksum = reader.ReadUInt32();
            var correctChecksum = Crc32CAlgorithm.Compute(header);
            if (checksum != correctChecksum)
                throw new FileFormatException("Incorrect header checksum");
        }

        private static void ReadPackageMetadata(BinaryReader reader, long offset)
        {
            reader.BaseStream.Position = offset;

            int? entryFunctionID = reader.ReadInt32();
            if (entryFunctionID < 0)
                entryFunctionID = null;
        }
    }
}
