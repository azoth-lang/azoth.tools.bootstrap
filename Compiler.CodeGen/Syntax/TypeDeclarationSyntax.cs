using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class TypeDeclarationSyntax
{
    public bool IsValueType { get; }
    public SymbolSyntax Name { get; }
    public IFixedList<SymbolSyntax> Supertypes { get; }

    public TypeDeclarationSyntax(bool isValueType, SymbolSyntax name, IEnumerable<SymbolSyntax> supertypes)
    {
        IsValueType = isValueType;
        Name = name;
        Supertypes = supertypes.ToFixedList();
    }
}
