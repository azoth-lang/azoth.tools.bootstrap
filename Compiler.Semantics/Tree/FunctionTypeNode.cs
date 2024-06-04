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
    private ValueAttribute<IMaybeAntetype> antetype;
    public override IMaybeAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, TypeExpressionsAntetypesAspect.FunctionType_Antetype);
    private ValueAttribute<DataType> type;
    public override DataType Type
    => type.TryGetValue(out var value) ? value
        : type.GetValue(this, TypeExpressionsAspect.FunctionType_Type);

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
