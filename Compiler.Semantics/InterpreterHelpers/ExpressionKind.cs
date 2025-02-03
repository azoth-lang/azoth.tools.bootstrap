namespace Azoth.Tools.Bootstrap.Compiler.Semantics.InterpreterHelpers;

public enum ExpressionKind
{
    /// <summary>
    /// A node that can only exist in a program with errors.
    /// </summary>
    Invalid = -1,

    NotTraversed = 1,

    #region Expressions
    Block,
    Unsafe,
    #endregion

    #region Instance Member Access Expressions
    FieldAccess,
    MethodAccess,
    #endregion

    #region Literal Expressions
    BoolLiteral,
    IntegerLiteral,
    NoneLiteral,
    StringLiteral,
    #endregion

    #region Operator Expressions
    Assignment,
    BinaryOperator,
    UnaryOperator,
    Conversion,
    ImplicitConversion,
    PatternMatch,
    Ref,
    ImplicitDeref,
    #endregion

    #region Control Flow Expressions
    If,
    Loop,
    While,
    Foreach,
    Break,
    Next,
    Return,
    #endregion

    #region Invocation Expressions
    FunctionInvocation,
    MethodInvocation,
    GetterInvocation,
    SetterInvocation,
    FunctionReferenceInvocation,
    InitializerInvocation,
    #endregion

    #region Name Expressions
    VariableName,
    Self,
    FunctionName,
    InitializerName,
    #endregion

    #region Capability Expressions
    Recovery,
    ImplicitTempMove,
    PrepareToReturn,
    #endregion

    #region Async Expressions
    AsyncBlock,
    AsyncStart,
    Await,
    #endregion
}
