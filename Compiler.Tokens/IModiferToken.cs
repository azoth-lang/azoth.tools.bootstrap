using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IAccessModifierToken),
    typeof(IMutableKeywordToken),
    typeof(IIsolatedKeywordToken),
    typeof(IConstKeywordToken),
    typeof(IMoveKeywordToken),
    typeof(ISafeKeywordToken),
    typeof(IUnsafeKeywordToken)
)]
public partial interface IModiferToken : IKeywordToken { }

public partial interface IAccessModifierToken : IModiferToken { }
public partial interface IMutableKeywordToken : IModiferToken { }
public partial interface IIsolatedKeywordToken : IModiferToken { }
public partial interface IConstKeywordToken : IModiferToken { }
public partial interface IMoveKeywordToken : IModiferToken { }
public partial interface ISafeKeywordToken : IModiferToken { }
public partial interface IUnsafeKeywordToken : IModiferToken { }
