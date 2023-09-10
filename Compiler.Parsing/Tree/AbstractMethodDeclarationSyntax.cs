using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class AbstractMethodDeclarationSyntax : MethodDeclarationSyntax
{
    public AbstractMethodDeclarationSyntax(
        IClassDeclarationSyntax declaringClass,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        Name name,
        ISelfParameterSyntax selfParameter,
        FixedList<INamedParameterSyntax> parameters,
        ITypeSyntax? returnType)
        : base(declaringClass, span, file, accessModifier, nameSpan, name,
            selfParameter, parameters, returnType)
    {
    }

    public override string ToString()
    {
        var returnType = ReturnType is not null ? " -> " + ReturnType : "";
        return $"fn {Name}({string.Join(", ", Parameters.Prepend<IParameterSyntax>(SelfParameter))}){returnType};";
    }
}
