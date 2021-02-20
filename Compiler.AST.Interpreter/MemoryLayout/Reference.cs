using System;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout
{
    /// <summary>
    /// A reference in Azoth IL. Incorporates both an object and its vtable reference.
    /// </summary>
    internal readonly struct Reference : IEquatable<Reference>
    {
        public AzothObject Object { get; }

        public VTableRef VTableRef { get; }

        public Reference(AzothObject o, VTableRef vTable)
        {
            Object = o;
            VTableRef = vTable;
        }

        public bool Equals(Reference other)
        {
            return Object.Equals(other.Object) && VTableRef.Equals(other.VTableRef);
        }

        public override bool Equals(object? obj)
        {
            return obj is Reference other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Object, VTableRef);
        }

        public static bool operator ==(Reference left, Reference right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Reference left, Reference right)
        {
            return !left.Equals(right);
        }
    }
}
