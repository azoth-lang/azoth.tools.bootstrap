using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionReferenceInvocationNode : ExpressionNode, IFunctionReferenceInvocationNode
{
    public override IInvocationExpressionSyntax Syntax { get; }
    public IExpressionNode Expression { get; }
    public FunctionAntetype FunctionAntetype { get; }
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.FunctionReferenceInvocation_Antetype);
    private ValueAttribute<FunctionType> functionType;
    public FunctionType FunctionType
        => functionType.TryGetValue(out var value) ? value
            : functionType.GetValue(this, ExpressionTypesAspect.FunctionReferenceInvocation_FunctionType);
    private ValueAttribute<DataType> type;
    public override DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, ExpressionTypesAspect.FunctionReferenceInvocation_Type);

    public FunctionReferenceInvocationNode(
        IInvocationExpressionSyntax syntax,
        IExpressionNode expression,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        Expression = Child.Attach(this, expression);
        FunctionAntetype = (FunctionAntetype)expression.Antetype;
        Arguments = ChildList.Create(this, arguments);
    }
}
