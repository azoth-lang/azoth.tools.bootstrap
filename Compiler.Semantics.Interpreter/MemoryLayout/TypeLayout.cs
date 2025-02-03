using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal abstract class TypeLayout
{
    private readonly FrozenDictionary<FieldSymbol, int> fieldLayout;
    private readonly ConcurrentDictionary<IFieldDeclarationNode, int> fieldIndexes
        = new(ReferenceEqualityComparer.Instance);

    public TypeLayout(ITypeDefinitionNode typeDefinition)
    {
        var fields = typeDefinition.InclusiveMembers.OfType<IFieldDefinitionNode>();
        fieldLayout = fields.Enumerate().ToFrozenDictionary(x => x.Value.Symbol.Assigned(), x => x.Index + 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetIndex(IFieldDeclarationNode field)
        => fieldIndexes.GetOrAdd(field, Factory, fieldLayout);

    private static int Factory(IFieldDeclarationNode field, FrozenDictionary<FieldSymbol, int> fieldLayout)
        => fieldLayout[field.Symbol.Assigned()];

    public AzothValue[] CreateInstanceFields()
    {
        var size = fieldLayout.Count + 1;
        var fields = new AzothValue[size];
        fields[0] = AzothValue.TypeLayout(this);
        return fields;
    }
}
