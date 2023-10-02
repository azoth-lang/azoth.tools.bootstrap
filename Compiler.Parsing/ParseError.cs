using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

/// <summary>
/// Error Code Ranges:
/// 1001-1999: Lexical Errors
/// 2001-2999: Parsing Errors
/// 3001-3999: Type Errors
/// 4001-4999: Borrow Checking Errors
/// 5001-5999: Name Binding Errors
/// 6001-6999: Other Semantic Errors
/// </summary>
internal static class ParseError
{
    /// <summary>
    /// Special temporary error for language features that are not implemented. For that reason
    /// it breaks convention and uses error number 2000
    /// </summary>
    public static Diagnostic NotImplemented(CodeFile file, TextSpan span, string feature)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            2000, $"{feature} are not yet implemented");
    }

    public static Diagnostic IncompleteDeclaration(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2001, "Incomplete declaration");
    }

    public static Diagnostic UnexpectedToken(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2002, $"Unexpected token `{file.Code[span]}`");
    }

    public static Diagnostic MissingToken(CodeFile file, Type expected, IToken found)
    {
        return new(file, found.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2003, $"Expected `{expected.GetFriendlyName()}` found `{found.Text(file.Code)}`");
    }

    public static Diagnostic DeclarationNotAllowedInExternal(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2004, "Only function declarations are allowed in external blocks");
    }

    public static Diagnostic UnexpectedEndOfExpression(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2005, "Unexpected end of expression");
    }

    public static Diagnostic CantMoveOutOfExpression(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2006, "Can't move out of expression. Can only move out of variable");
    }

    public static Diagnostic ResultStatementInBody(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2007, "Result statements can't appear directly in function or method bodies. Must be in block expression");
    }

    public static Diagnostic ExtraSelfParameter(CodeFile file, in TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2008, "There can be only one self parameter to a method");
    }

    public static Diagnostic SelfParameterMustBeFirst(CodeFile file, in TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2009, "Self parameter must be the first parameter");
    }

    public static Diagnostic CantAssignIntoExpression(CodeFile file, in TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2010, "Expression can not appear on the left hand side of an assignment");
    }

    public static Diagnostic MissingType(CodeFile file, in TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2011, "Variable type and capability missing");
    }

    public static Diagnostic CantFreezeExpression(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2012, "Can't freeze expression. Can only freeze a variable");
    }

    public static Diagnostic AbstractAssociatedFunction(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2013, "An associated function cannot be abstract");
    }

    public static Diagnostic ConcreteMethodDeclaredAbstract(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2014, "A concrete method cannot be abstract");
    }

    public static Diagnostic AbstractMethodMissingAbstractModifier(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2015, "An abstract method must be declared `abstract`");
    }

    public static Diagnostic AssociatedFunctionMissingBody(CodeFile file, TextSpan span, Name name)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2016, $"Associated function `{name}` is missing a method body");
    }

    public static Diagnostic MissingSelfParameter(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2017, "Constructor is missing a self parameter");
    }

    public static Diagnostic LentId(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2018, "Cannot declare `lent id` capability, use `id` instead");
    }

    public static Diagnostic LentVarNotAllowed(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2019, "Parameter binding cannot be both `lent` and `var`");
    }

    public static Diagnostic LentFieldParameter(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2020, "Field parameter cannot be `lent`");
    }
}
