using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class MethodDefinitionSyntax : InvocableDefinitionSyntax, IMethodDefinitionSyntax
{
    public new IdentifierName Name { [DebuggerStepThrough] get; }
    public IMethodSelfParameterSyntax SelfParameter { [DebuggerStepThrough] get; }
    public new IFixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public abstract IReturnSyntax? Return { [DebuggerStepThrough] get; }

    protected MethodDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        IdentifierName name,
        IMethodSelfParameterSyntax selfParameter,
        IFixedList<INamedParameterSyntax> parameters)
        : base(span, file, accessModifier, nameSpan, name,
            parameters)
    {
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters;
    }
}
