using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Conversions;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class ExpressionSyntax : Syntax, IExpressionSyntax
{
    /// <summary>
    /// If an expression has been poisoned, then it is errored in some way
    /// and we won't report errors against it in the future. We may also
    /// skip it for some processing.
    /// </summary>
    public bool Poisoned { [DebuggerStepThrough] get; private set; }

    private DataType? dataType;
    [DisallowNull]
    public DataType? DataType
    {
        [DebuggerStepThrough]
        get => dataType;
        set
        {
            if (dataType is not null)
                throw new InvalidOperationException("Can't set type repeatedly");
            dataType = value ?? throw new ArgumentNullException(nameof(DataType),
                "Can't set type to null");
        }
    }

    private Conversion? cachedOnConversion = null;
    private DataType? cachedConvertedDataType = null;
    private ExpressionSemantics? cachedConvertedExpressionSemantics = null;
    public DataType? ConvertedDataType
    {
        get
        {
            var (convertedType, _) = ApplyConversion();
            return convertedType;
        }
    }

    public ExpressionSemantics? ConvertedSemantics
    {
        get
        {
            var (_, convertedSemantics) = ApplyConversion();
            return convertedSemantics;
        }
    }

    public virtual Conversion ImplicitConversion
    {
        [DebuggerStepThrough]
        get;
        private set;
    } = IdentityConversion.Instance;

    private ExpressionSemantics? semantics;
    [DisallowNull]
    public ExpressionSemantics? Semantics
    {
        [DebuggerStepThrough]
        get => semantics;
        set
        {
            if (semantics is not null)
                throw new InvalidOperationException("Can't set semantics repeatedly");
            semantics = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    protected ExpressionSyntax(TextSpan span, ExpressionSemantics? semantics = null)
        : base(span)
    {
        this.semantics = semantics;
    }

    public void Poison() => Poisoned = true;

    protected abstract OperatorPrecedence ExpressionPrecedence { get; }

    public void AddConversion(ChainedConversion conversion)
    {
        if (!conversion.IsChainedTo(ImplicitConversion))
            throw new InvalidOperationException("Cannot add conversion not chained to existing conversion.");
        ImplicitConversion = conversion;
    }

    public string ToGroupedString(OperatorPrecedence surroundingPrecedence)
        => surroundingPrecedence > ExpressionPrecedence ? $"({this})" : ToString();

    private (DataType?, ExpressionSemantics?) ApplyConversion()
    {
        if (DataType is null || Semantics is null) return (DataType, Semantics);

        if (cachedOnConversion != ImplicitConversion)
        {
            (cachedConvertedDataType, cachedConvertedExpressionSemantics) =
                ImplicitConversion.Apply(DataType, Semantics.Value);
            cachedOnConversion = ImplicitConversion;
        }

        return (cachedConvertedDataType, cachedConvertedExpressionSemantics);
    }
}
