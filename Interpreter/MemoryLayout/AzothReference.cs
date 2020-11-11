using System;
using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.Interpreter.MemoryLayout
{
    /// <summary>
    /// A reference in Azoth IL. Incorporates both an object and its vtable reference.
    /// </summary>
    public readonly struct AzothReference : IEquatable<AzothReference>
    {
        [SuppressMessage("Naming", "CA1720:Identifier contains type name",
            Justification = "This is an Azoth Object")]
        public AzothObject Object { get; }
        public VTableRef VTableRef { get; }

        public AzothReference(AzothObject o, VTableRef vTable)
        {
            Object = o;
            VTableRef = vTable;
        }

        public bool Equals(AzothReference other)
        {
            return Object.Equals(other.Object) && VTableRef.Equals(other.VTableRef);
        }

        public override bool Equals(object? obj)
        {
            return obj is AzothReference other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Object, VTableRef);
        }

        public static bool operator ==(AzothReference left, AzothReference right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AzothReference left, AzothReference right)
        {
            return !left.Equals(right);
        }
    }
}
