using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

[Closed(typeof(SynthesizedAttributeEquationModel))]
public abstract class EquationModel
{
    public static EquationModel Create(AspectModel aspect, EquationSyntax syntax)
        => syntax switch
        {
            SynthesizedAttributeEquationSyntax syn => new SynthesizedAttributeEquationModel(aspect, syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    public AspectModel Aspect { get; }
    public abstract EquationSyntax Syntax { get; }
    public InternalSymbol Node { get; set; }
    public abstract AttributeModel Attribute { get; }

    protected EquationModel(AspectModel aspect, SymbolSyntax node)
    {
        Aspect = aspect;
        Node = Symbol.CreateInternalFromSyntax(Aspect.Tree, node);
    }
}
