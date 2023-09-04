using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Primitives;

public static class Intrinsic
{
    public static readonly FixedSymbolTree SymbolTree = DefineIntrinsicSymbols();

    public static readonly FunctionSymbol MemAllocate =
        SymbolTree.Symbols.OfType<FunctionSymbol>().Single(f => f.Name.Text == "mem_allocate");

    public static readonly FunctionSymbol PrintUtf8 =
        SymbolTree.Symbols.OfType<FunctionSymbol>().Single(f => f.Name.Text == "print_utf8");

    private static FixedSymbolTree DefineIntrinsicSymbols()
    {
        var intrinsicsPackage = new PackageSymbol("intrinsics");
        var tree = new SymbolTreeBuilder(intrinsicsPackage);

        var intrinsicsNamespace = new NamespaceSymbol(intrinsicsPackage, "intrinsics");

        // params: length
        var memAllocate = new FunctionSymbol(intrinsicsNamespace, "mem_allocate", Params(DataType.Size), DataType.Size);
        tree.Add(memAllocate);

        // params: ptr
        var memDeallocate = new FunctionSymbol(intrinsicsNamespace, "mem_deallocate", Params(DataType.Size));
        tree.Add(memDeallocate);

        // params: from_ptr, to_ptr, length
        var memCopy = new FunctionSymbol(intrinsicsNamespace, "mem_copy", Params(DataType.Size, DataType.Size, DataType.Size));
        tree.Add(memCopy);

        // params: from_ptr, value
        var memSetByte = new FunctionSymbol(intrinsicsNamespace, "mem_set_byte", Params(DataType.Size, DataType.Byte));
        tree.Add(memSetByte);

        // params: ptr
        var memGetByte = new FunctionSymbol(intrinsicsNamespace, "mem_get_byte", Params(DataType.Size), DataType.Byte);
        tree.Add(memGetByte);

        // params: ptr, length
        var printUtf8 = new FunctionSymbol(intrinsicsNamespace, "print_utf8", Params(DataType.Size, DataType.Size));
        tree.Add(printUtf8);

        // params: ptr, length
        var readUtf8Line = new FunctionSymbol(intrinsicsNamespace, "read_utf8_line", Params(DataType.Size, DataType.Size), DataType.Size);
        tree.Add(readUtf8Line);

        return tree.Build();
    }

    private static FixedList<DataType> Params(params DataType[] types)
    {
        return types.ToFixedList();
    }
}
