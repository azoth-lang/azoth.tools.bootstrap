using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Xunit;
using static Azoth.Tools.Bootstrap.Compiler.Types.ReferenceCapability;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types;

[Trait("Category", "Types")]
public class OptionalTypeTests
{
    [Fact]
    public void Optional_reference_has_reference_semantics()
    {
        var optionalAny = new OptionalType(new AnyType(Isolated));

        Assert.Equal(TypeSemantics.Reference, optionalAny.Semantics);
    }

    [Fact]
    public void Optional_copy_type_has_copy_semantics()
    {
        var optionalBool = new OptionalType(DataType.Bool);

        Assert.Equal(TypeSemantics.CopyValue, optionalBool.Semantics);
    }

    /// <summary>
    /// The type `never?` has only one value, `none`. That value can be
    /// freely copied into any optional type, hence `never?` has copy
    /// semantics.
    /// </summary>
    [Fact]
    public void Optional_never_type_has_copy_semantics()
    {
        var optionalNever = new OptionalType(DataType.Never);

        Assert.Equal(TypeSemantics.CopyValue, optionalNever.Semantics);
    }

    /// <summary>
    /// The type `⧼unknown⧽?` is assignable into any optional type. Note though
    /// that it isn't assignable into non-optional types so it can't have
    /// never semantics. We assume it has the most lenient semantics possible
    /// for it, which is copy.
    /// </summary>
    [Fact]
    public void Optional_unknown_type_has_copy_semantics()
    {
        var optionalNever = new OptionalType(DataType.Unknown);

        Assert.Equal(TypeSemantics.CopyValue, optionalNever.Semantics);
    }

    [Fact(Skip = "There are no move types yet (and they can't be faked)")]
    public void Optional_move_type_has_move_semantics()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Cannot_have_optional_void_type()
    {
        Assert.Throws<ArgumentException>(() => new OptionalType(DataType.Void));
    }

    [Fact]
    public void With_equal_referent_are_equal()
    {
        var type1 = new OptionalType(ObjectType.Create(Mutable, "Package", Namespace("foo", "bar"), false, false, true, "Baz"));
        var type2 = new OptionalType(ObjectType.Create(Mutable, "Package", Namespace("foo", "bar"), false, false, true, "Baz"));

        Assert.Equal(type1, type2);
    }

    private static NamespaceName Namespace(params string[] segments)
    {
        return new NamespaceName(segments);
    }
}
