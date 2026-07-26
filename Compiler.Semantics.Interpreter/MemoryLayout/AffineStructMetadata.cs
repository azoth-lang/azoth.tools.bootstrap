using System.Diagnostics;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class AffineStructMetadata : TypeMetadata
{
    public IAffineStructDefinitionNode AffineStruct { [DebuggerStepThrough] get; }

    public AffineStructMetadata(IAffineStructDefinitionNode affineStruct) : base(affineStruct, 1)
    {
        AffineStruct = affineStruct;
    }

    [Inline(InlineBehavior.Remove)]
    public new Value[] CreateInstanceFields() => base.CreateInstanceFields();
}
