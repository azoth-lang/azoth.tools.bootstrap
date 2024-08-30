using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class TypeDeclarationModel : IEquatable<TypeDeclarationModel>
{
    public TypeDeclarationSyntax Syntax { get; }
    public bool IsValueType => Syntax.IsValueType;
    public ExternalSymbol Name { get; }
    public IFixedSet<ExternalSymbol> DeclaredSupertypes { get; }
    public IFixedSet<ExternalSymbol> Supertypes => supertypes.Value;
    private readonly Lazy<IFixedSet<ExternalSymbol>> supertypes;

    public TypeDeclarationModel(TreeModel tree, TypeDeclarationSyntax syntax)
    {
        Syntax = syntax;
        Name = Symbol.CreateExternalFromSyntax(tree, syntax.Name);
        DeclaredSupertypes = syntax.Supertypes.Select(s => Symbol.CreateExternalFromSyntax(tree, s)).ToFixedSet();
        supertypes = new(ComputeSupertypes);
    }

    private IFixedSet<ExternalSymbol> ComputeSupertypes()
        => DeclaredSupertypes
           .Concat(DeclaredSupertypes.SelectMany(s => s.TypeDeclaration?.Supertypes ?? []))
           .ToFixedSet();

    #region Equality
    public bool Equals(TypeDeclarationModel? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return IsValueType == other.IsValueType
               && Name.Equals(other.Name)
               && DeclaredSupertypes.Equals(other.DeclaredSupertypes);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is TypeDeclarationModel other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(IsValueType, Name, DeclaredSupertypes);
    #endregion
}
