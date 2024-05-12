using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ParameterTypeNode : CodeNode, IParameterTypeNode
{
    public override IParameterTypeSyntax Syntax { get; }
    public bool IsLent => Syntax.IsLent;
    public ITypeNode Referent { get; }
    private ValueAttribute<Parameter> parameter;
    public Parameter Parameter
        => parameter.TryGetValue(out var value) ? value
            : parameter.GetValue(this, TypeExpressionsAspect.ParameterType_Parameter);

    public ParameterTypeNode(IParameterTypeSyntax syntax, ITypeNode referent)
    {
        Syntax = syntax;
        Referent = Child.Attach(this, referent);
    }
}
