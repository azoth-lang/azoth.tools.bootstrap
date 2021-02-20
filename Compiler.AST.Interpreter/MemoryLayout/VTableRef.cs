using System;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout
{
    internal readonly struct VTableRef : IEquatable<VTableRef>
    {
        private readonly int index;

        public VTableRef(int index)
        {
            this.index = index;
        }

        public bool Equals(VTableRef other)
        {
            return index == other.index;
        }

        public override bool Equals(object? obj)
        {
            return obj is VTableRef other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(index);
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
