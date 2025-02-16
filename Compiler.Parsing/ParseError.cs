using System;
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
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            2000, $"{feature} are not yet implemented.");
    }

    public static Diagnostic IncompleteDeclaration(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2001, "Incomplete declaration.");
    }

    public static Diagnostic UnexpectedToken(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2002, $"Unexpected token `{file.Code[span]}`.");
    }

    public static Diagnostic MissingToken(CodeFile file, Type expected, IToken found)
    {
        return new(file, found.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2003, $"Expected `{expected.GetFriendlyName()}` found `{found.Text(file.Code)}`.");
    }

    public static Diagnostic DeclarationNotAllowedInExternal(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2004, "Only function declarations are allowed in external blocks.");
    }

    public static Diagnostic UnexpectedEndOfExpression(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2005, "Unexpected end of expression.");
    }

    public static Diagnostic ResultStatementInBody(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2007, "Result statements can't appear directly in function or method bodies. Must be in block expression.");
    }

    public static Diagnostic ExtraSelfParameter(CodeFile file, in TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2008, "There can be only one self parameter to a method.");
    }

    public static Diagnostic SelfParameterMustBeFirst(CodeFile file, in TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2009, "Self parameter must be the first parameter.");
    }

    public static Diagnostic MissingType(CodeFile file, in TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2011, "Variable type and capability are missing.");
    }

    public static Diagnostic AbstractAssociatedFunction(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2013, "An associated function cannot be abstract.");
    }

    public static Diagnostic ConcreteMethodDeclaredAbstract(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2014, "A concrete method cannot be abstract.");
    }

    public static Diagnostic AbstractMethodMissingAbstractModifier(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2015, "An abstract method must be declared `abstract`.");
    }

    public static Diagnostic AssociatedFunctionMissingBody(CodeFile file, TextSpan span, IdentifierName name)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2016, $"Associated function `{name}` is missing a method body.");
    }

    public static Diagnostic MissingSelfParameter(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2017, "Initializer is missing a self parameter.");
    }

    public static Diagnostic LentVarNotAllowed(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2018, "Parameter binding cannot be both `lent` and `var`.");
    }

    public static Diagnostic LentFieldParameter(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2019, "Field parameter cannot be `lent`.");
    }

    public static Diagnostic UnexpectedEndOfPattern(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2020, "Unexpected end of pattern.");
    }

    public static Diagnostic InvalidTempCapability(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2021, "This reference capability does not support `temp`.");
    }

    public static Diagnostic ExplicitRead(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2022, "Explicit `read` capability should not be used here. Omit capability for implicit read.");
    }

    public static Diagnostic MissingReturn(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2024, "Getter is missing a return type.");
    }

    public static Diagnostic GetterHasParameters(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2025, "Getter has parameters.");
    }

    public static Diagnostic SetterHasReturn(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2026, "Setter has return.");
    }

    public static Diagnostic SetterMissingParameter(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2027, "Setter is missing a parameter.");
    }

    public static Diagnostic SetterHasExtraParameters(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2028, "Setter has extra parameters. A setter should have only one named parameter.");
    }

    public static Diagnostic ConcreteAssociatedTypeDeclaredAbstract(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2019, "A concrete associated type cannot be abstract.");
    }

    public static Diagnostic AbstractAssociatedTypeMissingAbstractModifier(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2020, "An abstract associated type must be declared `abstract`.");
    }

    public static Diagnostic AbstractModifierInTrait(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing,
            2021, "An abstract associated type must be declared `abstract`.");
    }
}
