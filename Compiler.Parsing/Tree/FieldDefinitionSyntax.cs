using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class FieldDefinitionSyntax : MemberDefinitionSyntax, IFieldDefinitionSyntax
{
    public bool IsMutableBinding { get; }
    public new IdentifierName Name { get; }
    public ITypeSyntax Type { get; }
    public IExpressionSyntax? Initializer { [DebuggerStepThrough] get; }

    public FieldDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        bool mutableBinding,
        TextSpan nameSpan,
        IdentifierName name,
        ITypeSyntax type,
        IExpressionSyntax? initializer)
        : base(span, file, accessModifier, nameSpan, name)
    {
        IsMutableBinding = mutableBinding;
        Name = name;
        Type = type;
        Initializer = initializer!;
    }

    public override string ToString()
    {
        var result = $"{Name}: {Type}";
        if (Initializer is not null)
            result += Initializer.ToString();
        result += ";";
        return result;
    }
}
