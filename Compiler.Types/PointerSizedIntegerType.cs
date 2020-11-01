using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types
{
    /// <summary>
    /// Integer types whose exact bit length is architecture dependent and whose
    /// length matches that of pointers
    /// </summary>
    public sealed class PointerSizedIntegerType : IntegerType
    {
        internal new static readonly PointerSizedIntegerType Size = new PointerSizedIntegerType(SpecialTypeName.Size, false);
        internal new static readonly PointerSizedIntegerType Offset = new PointerSizedIntegerType(SpecialTypeName.Offset, true);

        public bool IsSigned { get; }
        public override bool IsKnown => true;

        private PointerSizedIntegerType(SpecialTypeName name, bool signed)
            : base(name)
        {
            IsSigned = signed;
        }
    }
}
