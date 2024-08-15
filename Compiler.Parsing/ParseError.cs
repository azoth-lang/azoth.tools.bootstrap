using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

/// <remarks><see cref="ErrorCodeRange"/> for the ranges of various kinds of error codes.</remarks>
internal static class ParseError
{
    /// <summary>
    /// Special temporary error for language features that are not implemented. For that reason
    /// it breaks convention and uses error number 2000
    /// </summary>
    public static Diagnostic NotImplemented(CodeFile file, TextSpan span, string feature)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)2000, (string)$"{feature} are not yet implemented.");
    }

    public static Diagnostic IncompleteDeclaration(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2001, (string)"Incomplete declaration.");
    }

    public static Diagnostic UnexpectedToken(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2002, (string)$"Unexpected token `{file.Code[span]}`.");
    }

    public static Diagnostic MissingToken(CodeFile file, Type expected, IToken found)
    {
        return new(file, found.Span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2003, (string)$"Expected `{expected.GetFriendlyName()}` found `{found.Text(file.Code)}`.");
    }

    public static Diagnostic DeclarationNotAllowedInExternal(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2004, (string)"Only function declarations are allowed in external blocks.");
    }

    public static Diagnostic UnexpectedEndOfExpression(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2005, (string)"Unexpected end of expression.");
    }

    public static Diagnostic CantMoveOutOfExpression(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2006, (string)"Can't move out of expression. Can only move out of variable or self.");
    }

    public static Diagnostic ResultStatementInBody(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2007, (string)"Result statements can't appear directly in function or method bodies. Must be in block expression.");
    }

    public static Diagnostic ExtraSelfParameter(CodeFile file, in TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2008, (string)"There can be only one self parameter to a method.");
    }

    public static Diagnostic SelfParameterMustBeFirst(CodeFile file, in TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2009, (string)"Self parameter must be the first parameter.");
    }

    public static Diagnostic CantAssignIntoExpression(CodeFile file, in TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2010, (string)"Expression can not appear on the left hand side of an assignment.");
    }

    public static Diagnostic MissingType(CodeFile file, in TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2011, (string)"Variable type and capability are missing.");
    }

    public static Diagnostic CantFreezeExpression(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2012, (string)"Can't freeze expression. Can only freeze a variable or self.");
    }

    public static Diagnostic AbstractAssociatedFunction(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2013, (string)"An associated function cannot be abstract.");
    }

    public static Diagnostic ConcreteMethodDeclaredAbstract(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2014, (string)"A concrete method cannot be abstract.");
    }

    public static Diagnostic AbstractMethodMissingAbstractModifier(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2015, (string)"An abstract method must be declared `abstract`.");
    }

    public static Diagnostic AssociatedFunctionMissingBody(CodeFile file, TextSpan span, IdentifierName name)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2016, (string)$"Associated function `{name}` is missing a method body.");
    }

    public static Diagnostic MissingSelfParameter(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2017, (string)"Constructor is missing a self parameter.");
    }

    public static Diagnostic LentVarNotAllowed(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2018, (string)"Parameter binding cannot be both `lent` and `var`.");
    }

    public static Diagnostic LentFieldParameter(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2019, (string)"Field parameter cannot be `lent`.");
    }

    public static Diagnostic UnexpectedEndOfPattern(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2020, (string)"Unexpected end of pattern.");
    }

    public static Diagnostic InvalidTempCapability(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2021, (string)"This reference capability does not support `temp`.");
    }

    public static Diagnostic ExplicitRead(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2022, (string)"Explicit `read` capability should not be used here. Omit capability for implicit read.");
    }

    public static Diagnostic StructMethodMissingBody(CodeFile file, TextSpan span, IdentifierName name)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2023, (string)$"Method `{name}` is missing a method body.");
    }

    public static Diagnostic MissingReturn(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2024, (string)"Getter is missing a return type.");
    }

    public static Diagnostic GetterHasParameters(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2025, (string)"Getter has parameters.");
    }

    public static Diagnostic SetterHasReturn(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2026, (string)"Setter has return.");
    }

    public static Diagnostic SetterMissingParameter(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2027, (string)"Setter is missing a parameter.");
    }

    public static Diagnostic SetterHasExtraParameters(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Parsing,
            (int)2028, (string)"Setter has extra parameters. A setter should have only one named parameter.");
    }
}
