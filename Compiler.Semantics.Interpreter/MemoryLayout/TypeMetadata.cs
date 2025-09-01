using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

/// <summary>
/// Metadata about a type describing its memory layout and member lookup
/// </summary>
internal abstract class TypeMetadata
{
    private readonly int fieldOffset;
    private readonly FrozenDictionary<FieldSymbol, int> fieldLayout;
    private readonly ConcurrentDictionary<IFieldDeclarationNode, int> fieldIndexes
        = new(ReferenceEqualityComparer.Instance);

    protected TypeMetadata(ITypeDefinitionNode typeDefinition, int fieldOffset)
    {
        this.fieldOffset = fieldOffset;
        var fields = typeDefinition.InclusiveMembers.OfType<IFieldDefinitionNode>();
        fieldLayout = fields.Enumerate().ToFrozenDictionary(x => x.Value.Symbol.Assigned(), x => x.Index + fieldOffset);
    }

    [Inline(InlineBehavior.Remove)]
    public int GetIndex(IFieldDeclarationNode field)
        => fieldIndexes.GetOrAdd(field, Factory, fieldLayout);

    private static int Factory(IFieldDeclarationNode field, FrozenDictionary<FieldSymbol, int> fieldLayout)
        => fieldLayout[field.Symbol.Assigned()];

    protected Value[] CreateInstanceFields()
    {
        var size = fieldLayout.Count + fieldOffset;
        var fields = new Value[size];
        fields[0] = Value.From(this);
        return fields;
    }
}
