using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(typeof(IClassKeywordToken), typeof(ITraitKeywordToken), typeof(IStructKeywordToken))]
public interface ITypeKindKeywordToken : IKeywordToken;

public partial interface IClassKeywordToken : ITypeKindKeywordToken;
public partial interface ITraitKeywordToken : ITypeKindKeywordToken;
public partial interface IStructKeywordToken : ITypeKindKeywordToken;
