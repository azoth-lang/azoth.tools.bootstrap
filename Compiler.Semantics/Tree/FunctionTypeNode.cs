using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionTypeNode : TypeNode, IFunctionTypeNode
{
    public override IFunctionTypeSyntax Syntax { get; }
    public IFixedList<IParameterTypeNode> Parameters { get; }
    public ITypeNode Return { get; }
    private ValueAttribute<IMaybeAntetype> namedAntetype;
    public override IMaybeAntetype NamedAntetype
        => namedAntetype.TryGetValue(out var value) ? value
            : namedAntetype.GetValue(this, TypeExpressionsAntetypesAspect.FunctionType_NamedAntetype);
    private DataType? namedType;
    private bool namedTypeCached;
    public override DataType NamedType
        => GrammarAttribute.IsCached(in namedTypeCached) ? namedType!
            : this.Synthetic(ref namedTypeCached, ref namedType, TypeExpressionsAspect.FunctionType_NamedType);

    public FunctionTypeNode(
        IFunctionTypeSyntax syntax,
        IEnumerable<IParameterTypeNode> parameters,
        ITypeNode @return)
    {
        Syntax = syntax;
        Parameters = ChildList.Attach(this, parameters);
        Return = Child.Attach(this, @return);
    }
}
