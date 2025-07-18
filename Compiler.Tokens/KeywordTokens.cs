using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

public static partial class TokenTypes
{
    private static readonly IReadOnlyList<Type> Keyword = new List<Type>()
    {
        typeof(PublishedKeywordToken),
        typeof(PublicKeywordToken),
        typeof(ProtectedKeywordToken),
        typeof(LetKeywordToken),
        typeof(VarKeywordToken),
        typeof(VoidKeywordToken),
        typeof(IntKeywordToken),
        typeof(Int8KeywordToken),
        typeof(Int16KeywordToken),
        typeof(Int32KeywordToken),
        typeof(Int64KeywordToken),
        typeof(UIntKeywordToken),
        typeof(ByteKeywordToken),
        typeof(UInt16KeywordToken),
        typeof(UInt32KeywordToken),
        typeof(UInt64KeywordToken),
        typeof(SizeKeywordToken),
        typeof(OffsetKeywordToken),
        typeof(NIntKeywordToken),
        typeof(NUIntKeywordToken),
        typeof(BoolKeywordToken),
        typeof(NeverKeywordToken),
        typeof(ReturnKeywordToken),
        typeof(ClassKeywordToken),
        typeof(FunctionKeywordToken),
        typeof(InitKeywordToken),
        typeof(IsolatedKeywordToken),
        typeof(ReadKeywordToken),
        typeof(ConstKeywordToken),
        typeof(IdKeywordToken),
        typeof(LentKeywordToken),
        typeof(NamespaceKeywordToken),
        typeof(ImportKeywordToken),
        typeof(ForeachKeywordToken),
        typeof(InKeywordToken),
        typeof(IfKeywordToken),
        typeof(ElseKeywordToken),
        typeof(StructKeywordToken),
        typeof(UnsafeKeywordToken),
        typeof(SafeKeywordToken),
        typeof(SelfKeywordToken),
        typeof(SelfTypeKeywordToken),
        typeof(BaseKeywordToken),
        typeof(TypeKeywordToken),
        typeof(MutableKeywordToken),
        typeof(VariableRefKeywordToken),
        typeof(InternalRefKeywordToken),
        typeof(AbstractKeywordToken),
        typeof(GetKeywordToken),
        typeof(SetKeywordToken),
        typeof(NoneKeywordToken),
        typeof(MoveKeywordToken),
        typeof(FreezeKeywordToken),
        typeof(CopyKeywordToken),
        typeof(LoopKeywordToken),
        typeof(WhileKeywordToken),
        typeof(BreakKeywordToken),
        typeof(NextKeywordToken),
        typeof(AnyTypeKeywordToken),
        typeof(TrueKeywordToken),
        typeof(FalseKeywordToken),
        typeof(AsKeywordToken),
        typeof(AsExclamationKeywordToken),
        typeof(AsQuestionKeywordToken),
        typeof(AndKeywordToken),
        typeof(OrKeywordToken),
        typeof(NotKeywordToken),
        typeof(TraitKeywordToken),
        typeof(IsKeywordToken),
        typeof(AsyncKeywordToken),
        typeof(GoKeywordToken),
        typeof(DoKeywordToken),
        typeof(AwaitKeywordToken),
        typeof(OutKeywordToken),
        typeof(IndependentKeywordToken),
        typeof(TempKeywordToken),
        typeof(ReadableKeywordToken),
        typeof(ShareableKeywordToken),
        typeof(AliasableKeywordToken),
        typeof(SendableKeywordToken),
        typeof(TemporaryKeywordToken),
        typeof(AnyKeywordToken),
        typeof(NonwritableKeywordToken),
    }.AsReadOnly();
}

public static partial class TokenFactory
{
    public static IPublishedKeywordToken PublishedKeyword(TextSpan span)
        => new PublishedKeywordToken(span);

    public static IPublicKeywordToken PublicKeyword(TextSpan span)
        => new PublicKeywordToken(span);

    public static IProtectedKeywordToken ProtectedKeyword(TextSpan span)
        => new ProtectedKeywordToken(span);

    public static ILetKeywordToken LetKeyword(TextSpan span)
        => new LetKeywordToken(span);

    public static IVarKeywordToken VarKeyword(TextSpan span)
        => new VarKeywordToken(span);

    public static IVoidKeywordToken VoidKeyword(TextSpan span)
        => new VoidKeywordToken(span);

    public static IIntKeywordToken IntKeyword(TextSpan span)
        => new IntKeywordToken(span);

    public static IInt8KeywordToken Int8Keyword(TextSpan span)
        => new Int8KeywordToken(span);

    public static IInt16KeywordToken Int16Keyword(TextSpan span)
        => new Int16KeywordToken(span);

    public static IInt32KeywordToken Int32Keyword(TextSpan span)
        => new Int32KeywordToken(span);

    public static IInt64KeywordToken Int64Keyword(TextSpan span)
        => new Int64KeywordToken(span);

    public static IUIntKeywordToken UIntKeyword(TextSpan span)
        => new UIntKeywordToken(span);

    public static IByteKeywordToken ByteKeyword(TextSpan span)
        => new ByteKeywordToken(span);

    public static IUInt16KeywordToken UInt16Keyword(TextSpan span)
        => new UInt16KeywordToken(span);

    public static IUInt32KeywordToken UInt32Keyword(TextSpan span)
        => new UInt32KeywordToken(span);

    public static IUInt64KeywordToken UInt64Keyword(TextSpan span)
        => new UInt64KeywordToken(span);

    public static ISizeKeywordToken SizeKeyword(TextSpan span)
        => new SizeKeywordToken(span);

    public static IOffsetKeywordToken OffsetKeyword(TextSpan span)
        => new OffsetKeywordToken(span);

    public static INIntKeywordToken NIntKeyword(TextSpan span)
        => new NIntKeywordToken(span);

    public static INUIntKeywordToken NUIntKeyword(TextSpan span)
        => new NUIntKeywordToken(span);

    public static IBoolKeywordToken BoolKeyword(TextSpan span)
        => new BoolKeywordToken(span);

    public static INeverKeywordToken NeverKeyword(TextSpan span)
        => new NeverKeywordToken(span);

    public static IReturnKeywordToken ReturnKeyword(TextSpan span)
        => new ReturnKeywordToken(span);

    public static IClassKeywordToken ClassKeyword(TextSpan span)
        => new ClassKeywordToken(span);

    public static IFunctionKeywordToken FunctionKeyword(TextSpan span)
        => new FunctionKeywordToken(span);

    public static IInitKeywordToken InitKeyword(TextSpan span)
        => new InitKeywordToken(span);

    public static IIsolatedKeywordToken IsolatedKeyword(TextSpan span)
        => new IsolatedKeywordToken(span);

    public static IReadKeywordToken ReadKeyword(TextSpan span)
        => new ReadKeywordToken(span);

    public static IConstKeywordToken ConstKeyword(TextSpan span)
        => new ConstKeywordToken(span);

    public static IIdKeywordToken IdKeyword(TextSpan span)
        => new IdKeywordToken(span);

    public static ILentKeywordToken LentKeyword(TextSpan span)
        => new LentKeywordToken(span);

    public static INamespaceKeywordToken NamespaceKeyword(TextSpan span)
        => new NamespaceKeywordToken(span);

    public static IImportKeywordToken ImportKeyword(TextSpan span)
        => new ImportKeywordToken(span);

    public static IForeachKeywordToken ForeachKeyword(TextSpan span)
        => new ForeachKeywordToken(span);

    public static IInKeywordToken InKeyword(TextSpan span)
        => new InKeywordToken(span);

    public static IIfKeywordToken IfKeyword(TextSpan span)
        => new IfKeywordToken(span);

    public static IElseKeywordToken ElseKeyword(TextSpan span)
        => new ElseKeywordToken(span);

    public static IStructKeywordToken StructKeyword(TextSpan span)
        => new StructKeywordToken(span);

    public static IUnsafeKeywordToken UnsafeKeyword(TextSpan span)
        => new UnsafeKeywordToken(span);

    public static ISafeKeywordToken SafeKeyword(TextSpan span)
        => new SafeKeywordToken(span);

    public static ISelfKeywordToken SelfKeyword(TextSpan span)
        => new SelfKeywordToken(span);

    public static ISelfTypeKeywordToken SelfTypeKeyword(TextSpan span)
        => new SelfTypeKeywordToken(span);

    public static IBaseKeywordToken BaseKeyword(TextSpan span)
        => new BaseKeywordToken(span);

    public static ITypeKeywordToken TypeKeyword(TextSpan span)
        => new TypeKeywordToken(span);

    public static IMutableKeywordToken MutableKeyword(TextSpan span)
        => new MutableKeywordToken(span);

    public static IVariableRefKeywordToken VariableRefKeyword(TextSpan span)
        => new VariableRefKeywordToken(span);

    public static IInternalRefKeywordToken InternalRefKeyword(TextSpan span)
        => new InternalRefKeywordToken(span);

    public static IAbstractKeywordToken AbstractKeyword(TextSpan span)
        => new AbstractKeywordToken(span);

    public static IGetKeywordToken GetKeyword(TextSpan span)
        => new GetKeywordToken(span);

    public static ISetKeywordToken SetKeyword(TextSpan span)
        => new SetKeywordToken(span);

    public static INoneKeywordToken NoneKeyword(TextSpan span)
        => new NoneKeywordToken(span);

    public static IMoveKeywordToken MoveKeyword(TextSpan span)
        => new MoveKeywordToken(span);

    public static IFreezeKeywordToken FreezeKeyword(TextSpan span)
        => new FreezeKeywordToken(span);

    public static ICopyKeywordToken CopyKeyword(TextSpan span)
        => new CopyKeywordToken(span);

    public static ILoopKeywordToken LoopKeyword(TextSpan span)
        => new LoopKeywordToken(span);

    public static IWhileKeywordToken WhileKeyword(TextSpan span)
        => new WhileKeywordToken(span);

    public static IBreakKeywordToken BreakKeyword(TextSpan span)
        => new BreakKeywordToken(span);

    public static INextKeywordToken NextKeyword(TextSpan span)
        => new NextKeywordToken(span);

    public static IAnyTypeKeywordToken AnyTypeKeyword(TextSpan span)
        => new AnyTypeKeywordToken(span);

    public static ITrueKeywordToken TrueKeyword(TextSpan span)
        => new TrueKeywordToken(span);

    public static IFalseKeywordToken FalseKeyword(TextSpan span)
        => new FalseKeywordToken(span);

    public static IAsKeywordToken AsKeyword(TextSpan span)
        => new AsKeywordToken(span);

    public static IAsExclamationKeywordToken AsExclamationKeyword(TextSpan span)
        => new AsExclamationKeywordToken(span);

    public static IAsQuestionKeywordToken AsQuestionKeyword(TextSpan span)
        => new AsQuestionKeywordToken(span);

    public static IAndKeywordToken AndKeyword(TextSpan span)
        => new AndKeywordToken(span);

    public static IOrKeywordToken OrKeyword(TextSpan span)
        => new OrKeywordToken(span);

    public static INotKeywordToken NotKeyword(TextSpan span)
        => new NotKeywordToken(span);

    public static ITraitKeywordToken TraitKeyword(TextSpan span)
        => new TraitKeywordToken(span);

    public static IIsKeywordToken IsKeyword(TextSpan span)
        => new IsKeywordToken(span);

    public static IAsyncKeywordToken AsyncKeyword(TextSpan span)
        => new AsyncKeywordToken(span);

    public static IGoKeywordToken GoKeyword(TextSpan span)
        => new GoKeywordToken(span);

    public static IDoKeywordToken DoKeyword(TextSpan span)
        => new DoKeywordToken(span);

    public static IAwaitKeywordToken AwaitKeyword(TextSpan span)
        => new AwaitKeywordToken(span);

    public static IOutKeywordToken OutKeyword(TextSpan span)
        => new OutKeywordToken(span);

    public static IIndependentKeywordToken IndependentKeyword(TextSpan span)
        => new IndependentKeywordToken(span);

    public static ITempKeywordToken TempKeyword(TextSpan span)
        => new TempKeywordToken(span);

    public static IReadableKeywordToken ReadableKeyword(TextSpan span)
        => new ReadableKeywordToken(span);

    public static IShareableKeywordToken ShareableKeyword(TextSpan span)
        => new ShareableKeywordToken(span);

    public static IAliasableKeywordToken AliasableKeyword(TextSpan span)
        => new AliasableKeywordToken(span);

    public static ISendableKeywordToken SendableKeyword(TextSpan span)
        => new SendableKeywordToken(span);

    public static ITemporaryKeywordToken TemporaryKeyword(TextSpan span)
        => new TemporaryKeywordToken(span);

    public static IAnyKeywordToken AnyKeyword(TextSpan span)
        => new AnyKeywordToken(span);

    public static INonwritableKeywordToken NonwritableKeyword(TextSpan span)
        => new NonwritableKeywordToken(span);

}

[Closed(
    typeof(IPublishedKeywordToken),
    typeof(IPublicKeywordToken),
    typeof(IProtectedKeywordToken),
    typeof(ILetKeywordToken),
    typeof(IVarKeywordToken),
    typeof(IVoidKeywordToken),
    typeof(IIntKeywordToken),
    typeof(IInt8KeywordToken),
    typeof(IInt16KeywordToken),
    typeof(IInt32KeywordToken),
    typeof(IInt64KeywordToken),
    typeof(IUIntKeywordToken),
    typeof(IByteKeywordToken),
    typeof(IUInt16KeywordToken),
    typeof(IUInt32KeywordToken),
    typeof(IUInt64KeywordToken),
    typeof(ISizeKeywordToken),
    typeof(IOffsetKeywordToken),
    typeof(INIntKeywordToken),
    typeof(INUIntKeywordToken),
    typeof(IBoolKeywordToken),
    typeof(INeverKeywordToken),
    typeof(IReturnKeywordToken),
    typeof(IClassKeywordToken),
    typeof(IFunctionKeywordToken),
    typeof(IInitKeywordToken),
    typeof(IIsolatedKeywordToken),
    typeof(IReadKeywordToken),
    typeof(IConstKeywordToken),
    typeof(IIdKeywordToken),
    typeof(ILentKeywordToken),
    typeof(INamespaceKeywordToken),
    typeof(IImportKeywordToken),
    typeof(IForeachKeywordToken),
    typeof(IInKeywordToken),
    typeof(IIfKeywordToken),
    typeof(IElseKeywordToken),
    typeof(IStructKeywordToken),
    typeof(IUnsafeKeywordToken),
    typeof(ISafeKeywordToken),
    typeof(ISelfKeywordToken),
    typeof(ISelfTypeKeywordToken),
    typeof(IBaseKeywordToken),
    typeof(ITypeKeywordToken),
    typeof(IMutableKeywordToken),
    typeof(IVariableRefKeywordToken),
    typeof(IInternalRefKeywordToken),
    typeof(IAbstractKeywordToken),
    typeof(IGetKeywordToken),
    typeof(ISetKeywordToken),
    typeof(INoneKeywordToken),
    typeof(IMoveKeywordToken),
    typeof(IFreezeKeywordToken),
    typeof(ICopyKeywordToken),
    typeof(ILoopKeywordToken),
    typeof(IWhileKeywordToken),
    typeof(IBreakKeywordToken),
    typeof(INextKeywordToken),
    typeof(IAnyTypeKeywordToken),
    typeof(ITrueKeywordToken),
    typeof(IFalseKeywordToken),
    typeof(IAsKeywordToken),
    typeof(IAsExclamationKeywordToken),
    typeof(IAsQuestionKeywordToken),
    typeof(IAndKeywordToken),
    typeof(IOrKeywordToken),
    typeof(INotKeywordToken),
    typeof(ITraitKeywordToken),
    typeof(IIsKeywordToken),
    typeof(IAsyncKeywordToken),
    typeof(IGoKeywordToken),
    typeof(IDoKeywordToken),
    typeof(IAwaitKeywordToken),
    typeof(IOutKeywordToken),
    typeof(IIndependentKeywordToken),
    typeof(ITempKeywordToken),
    typeof(IReadableKeywordToken),
    typeof(IShareableKeywordToken),
    typeof(IAliasableKeywordToken),
    typeof(ISendableKeywordToken),
    typeof(ITemporaryKeywordToken),
    typeof(IAnyKeywordToken),
    typeof(INonwritableKeywordToken))]
public partial interface IKeywordToken : IToken;


public partial interface IPublishedKeywordToken : IKeywordToken;
internal partial class PublishedKeywordToken : Token, IPublishedKeywordToken
{
    public PublishedKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IPublicKeywordToken : IKeywordToken;
internal partial class PublicKeywordToken : Token, IPublicKeywordToken
{
    public PublicKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IProtectedKeywordToken : IKeywordToken;
internal partial class ProtectedKeywordToken : Token, IProtectedKeywordToken
{
    public ProtectedKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface ILetKeywordToken : IKeywordToken;
internal partial class LetKeywordToken : Token, ILetKeywordToken
{
    public LetKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IVarKeywordToken : IKeywordToken;
internal partial class VarKeywordToken : Token, IVarKeywordToken
{
    public VarKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IVoidKeywordToken : IKeywordToken;
internal partial class VoidKeywordToken : Token, IVoidKeywordToken
{
    public VoidKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IIntKeywordToken : IKeywordToken;
internal partial class IntKeywordToken : Token, IIntKeywordToken
{
    public IntKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IInt8KeywordToken : IKeywordToken;
internal partial class Int8KeywordToken : Token, IInt8KeywordToken
{
    public Int8KeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IInt16KeywordToken : IKeywordToken;
internal partial class Int16KeywordToken : Token, IInt16KeywordToken
{
    public Int16KeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IInt32KeywordToken : IKeywordToken;
internal partial class Int32KeywordToken : Token, IInt32KeywordToken
{
    public Int32KeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IInt64KeywordToken : IKeywordToken;
internal partial class Int64KeywordToken : Token, IInt64KeywordToken
{
    public Int64KeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IUIntKeywordToken : IKeywordToken;
internal partial class UIntKeywordToken : Token, IUIntKeywordToken
{
    public UIntKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IByteKeywordToken : IKeywordToken;
internal partial class ByteKeywordToken : Token, IByteKeywordToken
{
    public ByteKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IUInt16KeywordToken : IKeywordToken;
internal partial class UInt16KeywordToken : Token, IUInt16KeywordToken
{
    public UInt16KeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IUInt32KeywordToken : IKeywordToken;
internal partial class UInt32KeywordToken : Token, IUInt32KeywordToken
{
    public UInt32KeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IUInt64KeywordToken : IKeywordToken;
internal partial class UInt64KeywordToken : Token, IUInt64KeywordToken
{
    public UInt64KeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface ISizeKeywordToken : IKeywordToken;
internal partial class SizeKeywordToken : Token, ISizeKeywordToken
{
    public SizeKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IOffsetKeywordToken : IKeywordToken;
internal partial class OffsetKeywordToken : Token, IOffsetKeywordToken
{
    public OffsetKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface INIntKeywordToken : IKeywordToken;
internal partial class NIntKeywordToken : Token, INIntKeywordToken
{
    public NIntKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface INUIntKeywordToken : IKeywordToken;
internal partial class NUIntKeywordToken : Token, INUIntKeywordToken
{
    public NUIntKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IBoolKeywordToken : IKeywordToken;
internal partial class BoolKeywordToken : Token, IBoolKeywordToken
{
    public BoolKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface INeverKeywordToken : IKeywordToken;
internal partial class NeverKeywordToken : Token, INeverKeywordToken
{
    public NeverKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IReturnKeywordToken : IKeywordToken;
internal partial class ReturnKeywordToken : Token, IReturnKeywordToken
{
    public ReturnKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IClassKeywordToken : IKeywordToken;
internal partial class ClassKeywordToken : Token, IClassKeywordToken
{
    public ClassKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IFunctionKeywordToken : IKeywordToken;
internal partial class FunctionKeywordToken : Token, IFunctionKeywordToken
{
    public FunctionKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IInitKeywordToken : IKeywordToken;
internal partial class InitKeywordToken : Token, IInitKeywordToken
{
    public InitKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IIsolatedKeywordToken : IKeywordToken;
internal partial class IsolatedKeywordToken : Token, IIsolatedKeywordToken
{
    public IsolatedKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IReadKeywordToken : IKeywordToken;
internal partial class ReadKeywordToken : Token, IReadKeywordToken
{
    public ReadKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IConstKeywordToken : IKeywordToken;
internal partial class ConstKeywordToken : Token, IConstKeywordToken
{
    public ConstKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IIdKeywordToken : IKeywordToken;
internal partial class IdKeywordToken : Token, IIdKeywordToken
{
    public IdKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface ILentKeywordToken : IKeywordToken;
internal partial class LentKeywordToken : Token, ILentKeywordToken
{
    public LentKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface INamespaceKeywordToken : IKeywordToken;
internal partial class NamespaceKeywordToken : Token, INamespaceKeywordToken
{
    public NamespaceKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IImportKeywordToken : IKeywordToken;
internal partial class ImportKeywordToken : Token, IImportKeywordToken
{
    public ImportKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IForeachKeywordToken : IKeywordToken;
internal partial class ForeachKeywordToken : Token, IForeachKeywordToken
{
    public ForeachKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IInKeywordToken : IKeywordToken;
internal partial class InKeywordToken : Token, IInKeywordToken
{
    public InKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IIfKeywordToken : IKeywordToken;
internal partial class IfKeywordToken : Token, IIfKeywordToken
{
    public IfKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IElseKeywordToken : IKeywordToken;
internal partial class ElseKeywordToken : Token, IElseKeywordToken
{
    public ElseKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IStructKeywordToken : IKeywordToken;
internal partial class StructKeywordToken : Token, IStructKeywordToken
{
    public StructKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IUnsafeKeywordToken : IKeywordToken;
internal partial class UnsafeKeywordToken : Token, IUnsafeKeywordToken
{
    public UnsafeKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface ISafeKeywordToken : IKeywordToken;
internal partial class SafeKeywordToken : Token, ISafeKeywordToken
{
    public SafeKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface ISelfKeywordToken : IKeywordToken;
internal partial class SelfKeywordToken : Token, ISelfKeywordToken
{
    public SelfKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface ISelfTypeKeywordToken : IKeywordToken;
internal partial class SelfTypeKeywordToken : Token, ISelfTypeKeywordToken
{
    public SelfTypeKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IBaseKeywordToken : IKeywordToken;
internal partial class BaseKeywordToken : Token, IBaseKeywordToken
{
    public BaseKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface ITypeKeywordToken : IKeywordToken;
internal partial class TypeKeywordToken : Token, ITypeKeywordToken
{
    public TypeKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IMutableKeywordToken : IKeywordToken;
internal partial class MutableKeywordToken : Token, IMutableKeywordToken
{
    public MutableKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IVariableRefKeywordToken : IKeywordToken;
internal partial class VariableRefKeywordToken : Token, IVariableRefKeywordToken
{
    public VariableRefKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IInternalRefKeywordToken : IKeywordToken;
internal partial class InternalRefKeywordToken : Token, IInternalRefKeywordToken
{
    public InternalRefKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IAbstractKeywordToken : IKeywordToken;
internal partial class AbstractKeywordToken : Token, IAbstractKeywordToken
{
    public AbstractKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IGetKeywordToken : IKeywordToken;
internal partial class GetKeywordToken : Token, IGetKeywordToken
{
    public GetKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface ISetKeywordToken : IKeywordToken;
internal partial class SetKeywordToken : Token, ISetKeywordToken
{
    public SetKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface INoneKeywordToken : IKeywordToken;
internal partial class NoneKeywordToken : Token, INoneKeywordToken
{
    public NoneKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IMoveKeywordToken : IKeywordToken;
internal partial class MoveKeywordToken : Token, IMoveKeywordToken
{
    public MoveKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IFreezeKeywordToken : IKeywordToken;
internal partial class FreezeKeywordToken : Token, IFreezeKeywordToken
{
    public FreezeKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface ICopyKeywordToken : IKeywordToken;
internal partial class CopyKeywordToken : Token, ICopyKeywordToken
{
    public CopyKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface ILoopKeywordToken : IKeywordToken;
internal partial class LoopKeywordToken : Token, ILoopKeywordToken
{
    public LoopKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IWhileKeywordToken : IKeywordToken;
internal partial class WhileKeywordToken : Token, IWhileKeywordToken
{
    public WhileKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IBreakKeywordToken : IKeywordToken;
internal partial class BreakKeywordToken : Token, IBreakKeywordToken
{
    public BreakKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface INextKeywordToken : IKeywordToken;
internal partial class NextKeywordToken : Token, INextKeywordToken
{
    public NextKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IAnyTypeKeywordToken : IKeywordToken;
internal partial class AnyTypeKeywordToken : Token, IAnyTypeKeywordToken
{
    public AnyTypeKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface ITrueKeywordToken : IKeywordToken;
internal partial class TrueKeywordToken : Token, ITrueKeywordToken
{
    public TrueKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IFalseKeywordToken : IKeywordToken;
internal partial class FalseKeywordToken : Token, IFalseKeywordToken
{
    public FalseKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IAsKeywordToken : IKeywordToken;
internal partial class AsKeywordToken : Token, IAsKeywordToken
{
    public AsKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IAsExclamationKeywordToken : IKeywordToken;
internal partial class AsExclamationKeywordToken : Token, IAsExclamationKeywordToken
{
    public AsExclamationKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IAsQuestionKeywordToken : IKeywordToken;
internal partial class AsQuestionKeywordToken : Token, IAsQuestionKeywordToken
{
    public AsQuestionKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IAndKeywordToken : IKeywordToken;
internal partial class AndKeywordToken : Token, IAndKeywordToken
{
    public AndKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IOrKeywordToken : IKeywordToken;
internal partial class OrKeywordToken : Token, IOrKeywordToken
{
    public OrKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface INotKeywordToken : IKeywordToken;
internal partial class NotKeywordToken : Token, INotKeywordToken
{
    public NotKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface ITraitKeywordToken : IKeywordToken;
internal partial class TraitKeywordToken : Token, ITraitKeywordToken
{
    public TraitKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IIsKeywordToken : IKeywordToken;
internal partial class IsKeywordToken : Token, IIsKeywordToken
{
    public IsKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IAsyncKeywordToken : IKeywordToken;
internal partial class AsyncKeywordToken : Token, IAsyncKeywordToken
{
    public AsyncKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IGoKeywordToken : IKeywordToken;
internal partial class GoKeywordToken : Token, IGoKeywordToken
{
    public GoKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IDoKeywordToken : IKeywordToken;
internal partial class DoKeywordToken : Token, IDoKeywordToken
{
    public DoKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IAwaitKeywordToken : IKeywordToken;
internal partial class AwaitKeywordToken : Token, IAwaitKeywordToken
{
    public AwaitKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IOutKeywordToken : IKeywordToken;
internal partial class OutKeywordToken : Token, IOutKeywordToken
{
    public OutKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IIndependentKeywordToken : IKeywordToken;
internal partial class IndependentKeywordToken : Token, IIndependentKeywordToken
{
    public IndependentKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface ITempKeywordToken : IKeywordToken;
internal partial class TempKeywordToken : Token, ITempKeywordToken
{
    public TempKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IReadableKeywordToken : IKeywordToken;
internal partial class ReadableKeywordToken : Token, IReadableKeywordToken
{
    public ReadableKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IShareableKeywordToken : IKeywordToken;
internal partial class ShareableKeywordToken : Token, IShareableKeywordToken
{
    public ShareableKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IAliasableKeywordToken : IKeywordToken;
internal partial class AliasableKeywordToken : Token, IAliasableKeywordToken
{
    public AliasableKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface ISendableKeywordToken : IKeywordToken;
internal partial class SendableKeywordToken : Token, ISendableKeywordToken
{
    public SendableKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface ITemporaryKeywordToken : IKeywordToken;
internal partial class TemporaryKeywordToken : Token, ITemporaryKeywordToken
{
    public TemporaryKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface IAnyKeywordToken : IKeywordToken;
internal partial class AnyKeywordToken : Token, IAnyKeywordToken
{
    public AnyKeywordToken(TextSpan span)
        : base(span)
    {
    }
}

public partial interface INonwritableKeywordToken : IKeywordToken;
internal partial class NonwritableKeywordToken : Token, INonwritableKeywordToken
{
    public NonwritableKeywordToken(TextSpan span)
        : base(span)
    {
    }
}
