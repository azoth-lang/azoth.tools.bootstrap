using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.AttributeFamilies;

public sealed class InheritedAttributeFamilySyntax : AttributeFamilySyntax
{
    public bool IsStable { get; }

    public InheritedAttributeFamilySyntax(bool isStable, string name, TypeSyntax type)
        : base(name, type)
    {
        IsStable = isStable;
    }

    public override string ToString()
    {
        var stable = IsStable ? "stable " : "";
        return $"↓ {stable}*.{Name} <: {Type}";
    }
}
