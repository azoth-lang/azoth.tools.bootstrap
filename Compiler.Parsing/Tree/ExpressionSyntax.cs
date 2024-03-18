using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Conversions;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class ExpressionSyntax : Syntax, IExpressionSyntax
{
    public abstract IPromise<DataType?> DataType { get; }

    private Conversion? cachedOnConversion = null;
    private DataType? cachedConvertedDataType = null;
    public DataType? ConvertedDataType => ApplyConversion();

    public virtual Conversion ImplicitConversion
    {
        [DebuggerStepThrough]
        get;
        private set;
    } = IdentityConversion.Instance;


    protected ExpressionSyntax(TextSpan span)
        : base(span)
    {
    }

    protected abstract OperatorPrecedence ExpressionPrecedence { get; }

    public void AddConversion(ChainedConversion conversion)
    {
        if (!conversion.IsChainedTo(ImplicitConversion))
            throw new InvalidOperationException("Cannot add conversion not chained to existing conversion.");
        ImplicitConversion = conversion;
    }

    public string ToGroupedString(OperatorPrecedence surroundingPrecedence)
        => surroundingPrecedence > ExpressionPrecedence ? $"({this})" : ToString();

    private DataType? ApplyConversion()
    {
        if (!DataType.IsFulfilled || DataType.Result is null) return null;

        if (cachedOnConversion != ImplicitConversion)
        {
            cachedConvertedDataType = ImplicitConversion.Apply(DataType.Assigned());
            cachedOnConversion = ImplicitConversion;
        }

        return cachedConvertedDataType;
    }
}
