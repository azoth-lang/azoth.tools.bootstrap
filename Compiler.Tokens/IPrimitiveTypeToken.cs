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
    typeof(IOffsetKeywordToken)
//typeof(IFloat32KeywordToken),
//typeof(IFloat64KeywordToken)
)]
public interface IPrimitiveTypeToken : IEssentialToken { }

public partial interface IVoidKeywordToken : IPrimitiveTypeToken { }
public partial interface INeverKeywordToken : IPrimitiveTypeToken { }
public partial interface IBoolKeywordToken : IPrimitiveTypeToken { }
public partial interface IAnyTypeKeywordToken : IPrimitiveTypeToken { }
//public partial interface ITypeKeywordToken : IPrimitiveTypeToken { }
public partial interface IIntKeywordToken : IPrimitiveTypeToken { }
public partial interface IUIntKeywordToken : IPrimitiveTypeToken { }
public partial interface IInt8KeywordToken : IPrimitiveTypeToken { }
public partial interface IByteKeywordToken : IPrimitiveTypeToken { }
public partial interface IInt16KeywordToken : IPrimitiveTypeToken { }
public partial interface IUInt16KeywordToken : IPrimitiveTypeToken { }
public partial interface IInt32KeywordToken : IPrimitiveTypeToken { }
public partial interface IUInt32KeywordToken : IPrimitiveTypeToken { }
public partial interface IInt64KeywordToken : IPrimitiveTypeToken { }
public partial interface IUInt64KeywordToken : IPrimitiveTypeToken { }
public partial interface ISizeKeywordToken : IPrimitiveTypeToken { }
public partial interface IOffsetKeywordToken : IPrimitiveTypeToken { }

//public partial interface IFloat32KeywordToken : IPrimitiveTypeToken { }
//public partial interface IFloat64KeywordToken : IPrimitiveTypeToken { }
