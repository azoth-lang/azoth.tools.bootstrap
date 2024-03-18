using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(typeof(INamedVariableSymbol), typeof(ISelfParameterSymbol), typeof(IFieldSymbol))]
public interface IBindingSymbol
{
    public PackageSymbol Package { get; }
    public Symbol ContainingSymbol { get; }
    public bool IsMutableBinding { get; }
    public bool IsLentBinding { get; }
    public SimpleName? Name { get; }
    public Pseudotype Type { get; }
}
