namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.InferredSyntax
{
    //internal class FunctionInvocationExpressionSyntax : IUnqualifiedInvocationExpressionSyntax
    //{
    //    private readonly FixedSet<FunctionSymbol> possibleReferents;
    //    public LexicalScope ContainingLexicalScope
    //    {
    //        [DebuggerStepThrough]
    //        get => throw new NotSupportedException($"{nameof(ContainingLexicalScope)} not supported on inferred {nameof(FunctionInvocationExpressionSyntax)}");
    //        [DebuggerStepThrough]
    //        set => throw new NotSupportedException($"{nameof(ContainingLexicalScope)} not supported on inferred {nameof(FunctionInvocationExpressionSyntax)}");
    //    }
    //    public TextSpan Span { [DebuggerStepThrough] get; }
    //    public NamespaceName Namespace { [DebuggerStepThrough] get; }
    //    public Name InvokedName { [DebuggerStepThrough] get; }
    //    public TextSpan InvokedNameSpan { [DebuggerStepThrough] get; }
    //    public FixedList<IExpressionSyntax> Arguments { [DebuggerStepThrough] get; }
    //    public Promise<FunctionSymbol?> ReferencedSymbol { [DebuggerStepThrough] get; } = new Promise<FunctionSymbol?>();
    //    IPromise<InvocableSymbol?> IInvocationExpressionSyntax.ReferencedSymbol => ReferencedSymbol;

    //    private DataType? dataType;
    //    [DisallowNull]
    //    public DataType? DataType
    //    {
    //        [DebuggerStepThrough]
    //        get => dataType;
    //        set
    //        {
    //            if (dataType != null)
    //                throw new InvalidOperationException("Can't set type repeatedly");
    //            dataType = value ?? throw new ArgumentNullException(nameof(DataType), "Can't set type to null");
    //        }
    //    }

    //    private ExpressionSemantics? valueSemantics;

    //    [DisallowNull]
    //    public ExpressionSemantics? Semantics
    //    {
    //        [DebuggerStepThrough]
    //        get => valueSemantics;
    //        set
    //        {
    //            if (valueSemantics != null)
    //                throw new InvalidOperationException("Can't set semantics repeatedly");
    //            valueSemantics = value ?? throw new ArgumentNullException(nameof(value));
    //        }
    //    }

    //    public FunctionInvocationExpressionSyntax(
    //        TextSpan span,
    //        NamespaceName ns,
    //        Name invokedName,
    //        TextSpan invokedNameSpan,
    //        FixedList<IExpressionSyntax> arguments,
    //        FixedSet<FunctionSymbol> possibleReferents)
    //    {
    //        this.possibleReferents = possibleReferents;
    //        InvokedName = invokedName;
    //        InvokedNameSpan = invokedNameSpan;
    //        Span = span;
    //        Namespace = ns;
    //        Arguments = arguments;
    //    }

    //    public IEnumerable<IPromise<FunctionSymbol>> LookupInContainingScope()
    //    {
    //        return possibleReferents.Select(AcyclicPromise.ForValue);
    //    }

    //    public override string ToString()
    //    {
    //        return Namespace == NamespaceName.Global
    //            ? $"{InvokedName}({string.Join(", ", Arguments)})"
    //            : $"{Namespace}.{InvokedName}({string.Join(", ", Arguments)})";
    //    }

    //    public string ToGroupedString(OperatorPrecedence surroundingPrecedence)
    //    {
    //        return ToString();
    //    }
    //}
}
