using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

/// <summary>
/// Contextual keywords for type kinds.
/// </summary>
[Closed(
    typeof(IClassKeywordToken),
    typeof(IAffineKeywordToken),
    typeof(IStructKeywordToken),
    typeof(IValueKeywordToken),
    typeof(ITraitKeywordToken))]
public partial interface ITypeKindKeywordToken : IKeywordToken;

public partial interface IClassKeywordToken : ITypeKindKeywordToken;
public partial interface IAffineKeywordToken : ITypeKindKeywordToken;
public partial interface IStructKeywordToken : ITypeKindKeywordToken;
public partial interface IValueKeywordToken : ITypeKindKeywordToken;
public partial interface ITraitKeywordToken : ITypeKindKeywordToken;
