using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class GenericTypeNameSyntax : TypeSyntax, IGenericTypeNameSyntax
{
    public GenericName Name { get; }
    public IFixedList<ITypeSyntax> TypeArguments { get; }
    public BareType? NamedBareType => (NamedType as CapabilityType)?.BareType;

    public GenericTypeNameSyntax(TextSpan span, string name, IFixedList<ITypeSyntax> typeArguments)
        : base(span)
    {
        TypeArguments = typeArguments;
        Name = new GenericName(name, typeArguments.Count);
    }

    public override string ToString() => $"{Name.ToBareString()}[{string.Join(", ", TypeArguments)}]";
}
