using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IAccessModifierToken),
    typeof(IConstKeywordToken),
    typeof(IMoveKeywordToken),
    typeof(ISafeKeywordToken),
    typeof(IUnsafeKeywordToken),
    typeof(IAbstractKeywordToken),
    typeof(IStructKindKeywordToken))]
public partial interface IModifierToken : IKeywordToken { }

public partial interface IAccessModifierToken : IModifierToken { }
public partial interface IConstKeywordToken : IModifierToken { }
public partial interface IMoveKeywordToken : IModifierToken { }
public partial interface ISafeKeywordToken : IModifierToken { }
public partial interface IUnsafeKeywordToken : IModifierToken { }
public partial interface IAbstractKeywordToken : IModifierToken { }
public partial interface IStructKindKeywordToken : IModifierToken { }
