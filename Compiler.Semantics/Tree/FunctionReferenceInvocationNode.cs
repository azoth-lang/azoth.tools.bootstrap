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
    private FunctionType? functionType;
    private bool functionTypeCached;
    public FunctionType FunctionType
        => GrammarAttribute.IsCached(in functionTypeCached) ? functionType!
            : this.Synthetic(ref functionTypeCached, ref functionType, ExpressionTypesAspect.FunctionReferenceInvocation_FunctionType);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.FunctionReferenceInvocation_Type);

    public FunctionReferenceInvocationNode(
        IInvocationExpressionSyntax syntax,
        IExpressionNode expression,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        Expression = Child.Attach(this, expression);
        FunctionAntetype = (FunctionAntetype)expression.Antetype;
        Arguments = ChildList.CreateLegacy(this, arguments);
    }
}
