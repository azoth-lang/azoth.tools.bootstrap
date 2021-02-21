using System;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout
{
    internal readonly struct VTableRef : IEquatable<VTableRef>
    {
        public readonly int Index;

        public VTableRef(int index)
        {
            Index = index;
        }

        public bool Equals(VTableRef other)
        {
            return Index == other.Index;
        }

        public override bool Equals(object? obj)
        {
            return obj is VTableRef other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Index);
        }

        public static bool operator ==(VTableRef left, VTableRef right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VTableRef left, VTableRef right)
        {
            return !left.Equals(right);
        }
    }
}
