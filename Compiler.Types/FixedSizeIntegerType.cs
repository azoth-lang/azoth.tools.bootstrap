using System;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types
{
    public sealed class FixedSizeIntegerType : IntegerType
    {
        //internal new static readonly FixedSizeIntegerType Int8 = new FixedSizeIntegerType("int8", -8);
        internal new static readonly FixedSizeIntegerType Byte = new FixedSizeIntegerType(SpecialTypeName.Byte, 8);
        //internal new static readonly FixedSizeIntegerType Int16 = new FixedSizeIntegerType("int16", -16);
        //internal new static readonly FixedSizeIntegerType UInt16 = new FixedSizeIntegerType("uint16", 16);
        internal new static readonly FixedSizeIntegerType Int32 = new FixedSizeIntegerType(SpecialTypeName.Int, -32);
        internal new static readonly FixedSizeIntegerType UInt32 = new FixedSizeIntegerType(SpecialTypeName.UInt, 32);
        //internal new static readonly FixedSizeIntegerType Int64 = new FixedSizeIntegerType("int64", -64);
        //internal new static readonly FixedSizeIntegerType UInt64 = new FixedSizeIntegerType("uint64", 64);

        public bool IsSigned { get; }
        public int Bits { get; }
        public override bool IsKnown => true;

        private FixedSizeIntegerType(SpecialTypeName name, int bits)
            : base(name)
        {
            IsSigned = bits < 0;
            Bits = Math.Abs(bits);
        }
    }
}
