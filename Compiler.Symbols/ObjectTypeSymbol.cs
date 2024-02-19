using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// A symbol for a type declaration (i.e. a class or trait)
/// </summary>
public sealed class ObjectTypeSymbol : TypeSymbol
{
    public override PackageSymbol Package { get; }
    public override NamespaceOrPackageSymbol ContainingSymbol { get; }
    public override StandardTypeName Name { get; }
    public ObjectType DeclaresType { get; }

    public ObjectTypeSymbol(
        NamespaceOrPackageSymbol containingSymbol,
        ObjectType declaresType)
        : base(declaresType.Name)
    {
        // TODO check the declared type is in the containing namespace and package
        Package = containingSymbol.Package;
        ContainingSymbol = containingSymbol;
        Name = declaresType.Name;
        DeclaresType = declaresType;
    }

    #region Equals
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
    #endregion

    public override string ToILString()
    {
        var genericParametersCount = DeclaresType.GenericParameters.Count;
        var value = $"{ContainingSymbol.ToILString()}.{Name.ToBareString()}";
        if (genericParametersCount > 0)
            value += $"[{new string(',', genericParametersCount - 1)}]";
        return value;
    }
}
