using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Snippets;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Snippets;

public sealed class ConstructorArgumentValidationModel : SnippetModel
{
    public override ConstructorArgumentValidationSyntax Syntax { get; }

    public ConstructorArgumentValidationModel(AspectModel aspect, ConstructorArgumentValidationSyntax syntax)
        : base(aspect, syntax.Node)
    {
        Syntax = syntax;
    }

    public override string ToString() => $"+ {NodeSymbol}.new.Validate";
}
