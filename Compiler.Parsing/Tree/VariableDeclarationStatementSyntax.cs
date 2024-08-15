using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class VariableDeclarationStatementSyntax : StatementSyntax, IVariableDeclarationStatementSyntax
{
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public ITypeSyntax? Type { [DebuggerStepThrough] get; }
    public ICapabilitySyntax? Capability { [DebuggerStepThrough] get; }
    public IExpressionSyntax? Initializer { [DebuggerStepThrough] get; }

    public VariableDeclarationStatementSyntax(
        TextSpan span,
        bool isMutableBinding,
        IdentifierName name,
        TextSpan nameSpan,
        ITypeSyntax? typeSyntax,
        ICapabilitySyntax? capability,
        IExpressionSyntax? initializer)
        : base(span)
    {
        IsMutableBinding = isMutableBinding;
        Name = name;
        NameSpan = nameSpan;
        Type = typeSyntax;
        Capability = capability;
        Initializer = initializer;
    }

    public override string ToString()
    {
        var binding = IsMutableBinding ? "var" : "let";
        var type = Type is not null ? ": " + Type : "";
        if (Capability is not null) type = ": " + Capability;
        var initializer = Initializer is not null ? " = " + Initializer : "";
        return $"{binding} {Name}{type}{initializer};";
    }
}
