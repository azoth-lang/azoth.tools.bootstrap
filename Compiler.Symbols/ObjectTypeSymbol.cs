using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// A symbol for a type declaration (i.e. a class or trait)
/// </summary>
public sealed class ObjectTypeSymbol : TypeSymbol
{
    public override PackageSymbol? Package { get; }
    public new NamespaceOrPackageSymbol ContainingSymbol { get; }
    public new TypeName Name { get; }
    public BareObjectType DeclaresType { get; }

    public ObjectTypeSymbol(
        NamespaceOrPackageSymbol containingSymbol,
        BareObjectType declaresType)
        : base(containingSymbol, declaresType.Name)
    {
        // TODO check the declared type is in the containing namespace and package
        Package = containingSymbol.Package;
        ContainingSymbol = containingSymbol;
        Name = declaresType.Name;
        DeclaresType = declaresType;
    }

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ObjectTypeSymbol otherType
               && ContainingSymbol == otherType.ContainingSymbol
               && Name == otherType.Name
               && DeclaresType == otherType.DeclaresType;
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingSymbol, Name, DeclaresType);

    public override string ToILString()
        // TODO include generics
        => $"{ContainingSymbol.ToILString()}.{Name}";
}
