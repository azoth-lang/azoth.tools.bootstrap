using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(ILetKeywordToken),
    typeof(IVarKeywordToken))]
public interface IBindingToken : IKeywordToken;

public partial interface ILetKeywordToken : IBindingToken;
public partial interface IVarKeywordToken : IBindingToken;
