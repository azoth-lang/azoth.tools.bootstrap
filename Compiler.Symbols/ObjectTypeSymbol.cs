using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// A symbol for a type declaration (i.e. a class)
/// </summary>
public sealed class ObjectTypeSymbol : TypeSymbol
{
    public override PackageSymbol? Package { get; }
    public new NamespaceOrPackageSymbol ContainingSymbol { get; }
    public new TypeName Name { get; }
    public new ObjectType DeclaresDataType { get; }

    public ObjectTypeSymbol(
        NamespaceOrPackageSymbol containingSymbol,
        ObjectType declaresDataType)
        : base(containingSymbol, declaresDataType.Name, declaresDataType)
    {
        // TODO check the declared type is in the containing namespace and package
        Package = containingSymbol.Package;
        ContainingSymbol = containingSymbol;
        Name = declaresDataType.Name;
        DeclaresDataType = declaresDataType;
    }

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ObjectTypeSymbol otherType
               && ContainingSymbol == otherType.ContainingSymbol
               && Name == otherType.Name
               && DeclaresDataType == otherType.DeclaresDataType;
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingSymbol, Name, DeclaresDataType);

    public override string ToILString()
        // TODO include generics
        => $"{ContainingSymbol.ToILString()}.{Name}";
}
