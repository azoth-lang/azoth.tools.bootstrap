using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ParameterizedTypeSyntax : TypeSyntax, IParameterizedTypeSyntax
{
    public ITypeNameSyntax TypeName { get; }
    public Promise<TypeSymbol?> ReferencedSymbol { get; } = new Promise<TypeSymbol?>();
    public FixedList<ITypeSyntax> TypeArguments { get; }

    public ParameterizedTypeSyntax(TextSpan span, ITypeNameSyntax typeName, FixedList<ITypeSyntax> typeArguments)
        : base(span)
    {
        TypeArguments = typeArguments;
        TypeName = typeName;
    }

    public override string ToString() => $"{TypeName}[{string.Join(", ", TypeArguments)}]";
}
