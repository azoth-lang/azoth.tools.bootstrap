using System;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols
{
    /// <summary>
    /// A symbol a package
    /// </summary>
    /// <remarks>
    /// A package alias has no affect on the symbol. It is still the same package.
    /// </remarks>
    public class PackageSymbol : NamespaceOrPackageSymbol
    {
        public override PackageSymbol Package => this;

        public PackageSymbol(Name name)
            : base(null, name) { }

        public override bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is PackageSymbol otherNamespace
                   && Name == otherNamespace.Name;
        }

        public override int GetHashCode() => HashCode.Combine(Name);

        public override string ToILString() => $"<{Name}>::";
    }
}
