using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

[Closed(
    typeof(FieldNameExpressionSyntax),
    typeof(MethodGroupNameSyntax),
    typeof(GetterNameSyntax),
    typeof(SetterGroupNameSyntax),
    typeof(FunctionGroupNameSyntax),
    typeof(InitializerGroupNameSyntax),
    typeof(NamespaceNameSyntax),
    typeof(TypeNameSyntax),
    typeof(UnknownNameSyntax))]
public interface IMemberAccessSyntaxSemantics : ISyntaxSemantics
{
    IPromise<Symbol?> Symbol { get; }
}
