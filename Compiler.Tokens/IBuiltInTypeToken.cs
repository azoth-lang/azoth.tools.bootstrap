using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IVoidKeywordToken),
    typeof(INeverKeywordToken),
    typeof(IBoolKeywordToken),
    typeof(IAnyTypeKeywordToken),
    //typeof(ITypeKeywordToken),
    typeof(IIntKeywordToken),
    typeof(IUIntKeywordToken),
    typeof(IInt8KeywordToken),
    typeof(IByteKeywordToken),
    typeof(IInt16KeywordToken),
    typeof(IUInt16KeywordToken),
    typeof(IInt32KeywordToken),
    typeof(IUInt32KeywordToken),
    typeof(IInt64KeywordToken),
    typeof(IUInt64KeywordToken),
    typeof(ISizeKeywordToken),
    typeof(IOffsetKeywordToken),
    typeof(INIntKeywordToken),
    typeof(INUIntKeywordToken)
//typeof(IFloat32KeywordToken),
//typeof(IFloat64KeywordToken)
)]
public interface IBuiltInTypeToken : IEssentialToken;

public partial interface IVoidKeywordToken : IBuiltInTypeToken;
public partial interface INeverKeywordToken : IBuiltInTypeToken;
public partial interface IBoolKeywordToken : IBuiltInTypeToken;
public partial interface IAnyTypeKeywordToken : IBuiltInTypeToken;
//public partial interface ITypeKeywordToken : IBuiltInTypeToken;
public partial interface IIntKeywordToken : IBuiltInTypeToken;
public partial interface IUIntKeywordToken : IBuiltInTypeToken;
public partial interface IInt8KeywordToken : IBuiltInTypeToken;
public partial interface IByteKeywordToken : IBuiltInTypeToken;
public partial interface IInt16KeywordToken : IBuiltInTypeToken;
public partial interface IUInt16KeywordToken : IBuiltInTypeToken;
public partial interface IInt32KeywordToken : IBuiltInTypeToken;
public partial interface IUInt32KeywordToken : IBuiltInTypeToken;
public partial interface IInt64KeywordToken : IBuiltInTypeToken;
public partial interface IUInt64KeywordToken : IBuiltInTypeToken;
public partial interface ISizeKeywordToken : IBuiltInTypeToken;
public partial interface IOffsetKeywordToken : IBuiltInTypeToken;
public partial interface INIntKeywordToken : IBuiltInTypeToken;
public partial interface INUIntKeywordToken : IBuiltInTypeToken;
//public partial interface IFloat32KeywordToken : IBuiltInTypeToken;
//public partial interface IFloat64KeywordToken : IBuiltInTypeToken;
