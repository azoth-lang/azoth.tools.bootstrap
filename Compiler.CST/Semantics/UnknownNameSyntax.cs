using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

public sealed class UnknownNameSyntax : SyntaxSemantics,
    IIdentifierNameExpressionSyntaxSemantics, ISelfExpressionSyntaxSemantics, IMemberAccessSyntaxSemantics
{
    #region Singleton
    public static readonly UnknownNameSyntax Instance = new();

    private UnknownNameSyntax() : base(FixedSet.Empty<Symbol>()) { }
    #endregion

    IPromise<Symbol?> IMemberAccessSyntaxSemantics.Symbol => Promise.Null<Symbol>();
    IPromise<Symbol?> IIdentifierNameExpressionSyntaxSemantics.Symbol => Promise.Null<Symbol>();
    SelfParameterSymbol? ISelfExpressionSyntaxSemantics.Symbol => null;
    public override Promise<UnknownType> Type => DataType.PromiseOfUnknown;
    IPromise<Pseudotype> ISelfExpressionSyntaxSemantics.Pseudotype => DataType.PromiseOfUnknown;
}
