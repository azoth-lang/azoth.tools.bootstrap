using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class OptionalTypeNode : TypeNode, IOptionalTypeNode
{
    public override IOptionalTypeSyntax Syntax { get; }
    public ITypeNode Referent { get; }
    private ValueAttribute<IMaybeAntetype> antetype;
    public override IMaybeAntetype NamedAntetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, TypeExpressionsAntetypesAspect.OptionalType_NamedAntetype);
    private ValueAttribute<DataType> type;
    public override DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeExpressionsAspect.OptionalType_Type);

    public OptionalTypeNode(IOptionalTypeSyntax syntax, ITypeNode referent)
    {
        Syntax = syntax;
        Referent = Child.Attach(this, referent);
    }
}
