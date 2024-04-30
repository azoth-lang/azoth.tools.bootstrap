using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

public abstract record CreateMethod : Method
{
    public Rule ToRule { get; }
    public Symbol To => ToRule.Defines;
    public Type ReturnType { get; }
    public override bool ParametersDeclared => false;

    private protected CreateMethod(Pass pass, Rule toRule)
          : base(pass, toRule.ExtendsRule!.DefinesType)
    {
        Requires.That(nameof(toRule), toRule.Grammar == pass.ToLanguage?.Grammar, "Must be in the 'To' language");
        ToRule = toRule;
        ReturnType = toRule.DefinesType;
    }
}
