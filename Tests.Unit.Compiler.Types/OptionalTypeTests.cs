using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Xunit;
using static Azoth.Tools.Bootstrap.Compiler.Types.Capabilities.Capability;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types;

[Trait("Category", "Types")]
public class OptionalTypeTests
{
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
        var type1 = new OptionalType(ReferenceType.CreateClass(Mutable, "Package", Namespace("foo", "bar"), false, false, "Baz"));
        var type2 = new OptionalType(ReferenceType.CreateClass(Mutable, "Package", Namespace("foo", "bar"), false, false, "Baz"));

        Assert.Equal(type1, type2);
    }

    private static NamespaceName Namespace(params string[] segments)
    {
        return new NamespaceName(segments);
    }
}
