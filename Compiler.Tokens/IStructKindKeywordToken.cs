using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(typeof(ICopyKeywordToken), typeof(IMoveKeywordToken))]
public partial interface IStructKindKeywordToken : IModifierToken { }

public partial interface ICopyKeywordToken : IStructKindKeywordToken { }
public partial interface IMoveKeywordToken : IStructKindKeywordToken { }
