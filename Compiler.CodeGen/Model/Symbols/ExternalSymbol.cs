using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;

public sealed class ExternalSymbol : Symbol
{
    public override string FullName { get; }
    public TypeDeclarationModel? TypeDeclaration => typeDeclaration.Value;
    private readonly Lazy<TypeDeclarationModel?> typeDeclaration;
    public override bool IsValueType => TypeDeclaration?.IsValueType ?? PrimitiveTypes.Contains(FullName);

    public ExternalSymbol(TreeModel tree, string fullName)
    {
        Requires.NotNullOrEmpty(fullName, nameof(fullName));
        FullName = fullName;
        typeDeclaration = new(ComputeTypeDeclaration);
        return;

        TypeDeclarationModel? ComputeTypeDeclaration()
            => tree.TypeDeclarations.GetValueOrDefault(this);
    }

    #region Equality
    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ExternalSymbol symbol
               && FullName == symbol.FullName;
    }

    public override int GetHashCode() => HashCode.Combine(FullName);
    #endregion

    public override string ToString() => $"`{FullName}`";

    private static readonly IFixedSet<string> PrimitiveTypes = new HashSet<string>()
    {
        "bool","byte","sbyte","char","decimal","double","float","int","uint","long","ulong","short","ushort"
    }.ToFixedSet();
}
