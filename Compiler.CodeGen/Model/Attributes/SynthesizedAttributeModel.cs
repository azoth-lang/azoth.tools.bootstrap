using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public sealed class SynthesizedAttributeModel : AttributeModel
{
    public override SynthesizedAttributeSyntax Syntax { get; }
    public string Name => Syntax.Name;
    public string Parameters => Syntax.Parameters ?? "";
    public TypeModel Type { get; }
    public string? DefaultExpression => Syntax.DefaultExpression;

    public SynthesizedAttributeModel(AspectModel aspect, SynthesizedAttributeSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node))
    {
        Syntax = syntax;
        Type = TypeModel.CreateFromSyntax(Aspect.Tree, Syntax.Type);
    }
}
