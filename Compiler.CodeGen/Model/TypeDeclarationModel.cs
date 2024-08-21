using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class TypeDeclarationModel : IEquatable<TypeDeclarationModel>
{
    public TypeDeclarationSyntax Syntax { get; }
    public bool IsValueType => Syntax.IsValueType;
    public ExternalSymbol Name { get; }

    public TypeDeclarationModel(TreeModel tree, TypeDeclarationSyntax syntax)
    {
        Syntax = syntax;
        Name = Symbol.CreateExternalFromSyntax(tree, syntax.Name);
    }

    #region Equality
    public bool Equals(TypeDeclarationModel? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return IsValueType == other.IsValueType && Name.Equals(other.Name);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is TypeDeclarationModel other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(IsValueType, Name);
    #endregion
}
