using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols
{
    [Closed(
        typeof(NamedBindingSymbol),
        typeof(SelfParameterSymbol))]
    public abstract class BindingSymbol : Symbol
    {
        public override PackageSymbol? Package { get; }
        public new Name? Name { get; }
        public bool IsMutableBinding { get; }
        public DataType DataType { get; }

        protected BindingSymbol(Symbol containingSymbol, Name? name, bool isMutableBinding, DataType dataType)
            : base(containingSymbol, name)
        {
            Package = containingSymbol.Package;
            Name = name;
            IsMutableBinding = isMutableBinding;
            DataType = dataType;
        }
    }
}
