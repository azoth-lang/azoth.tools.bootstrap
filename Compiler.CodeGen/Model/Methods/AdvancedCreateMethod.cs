using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

/// <summary>
/// An advanced create method is one that will call `Transform` on child nodes.
/// </summary>
[Closed(
    typeof(AdvancedCreateTerminalMethod),
    typeof(AdvancedCreateNonTerminalMethod))]
public abstract record AdvancedCreateMethod : Method
{
    public Pass Pass { get; }
    public Rule Rule { get; }
    public Parameter From { get; }
    public Symbol To => Rule.Defines;
    public Type ReturnType { get; }

    private protected AdvancedCreateMethod(Pass pass, Rule rule)
        : base(rule.ExtendsRule!.DefinesType)
    {
        Pass = pass;
        Rule = rule;
        From = Parameter.Create(rule.ExtendsRule!.DefinesType, Parameter.FromName);
        ReturnType = rule.DefinesType;
    }
}
