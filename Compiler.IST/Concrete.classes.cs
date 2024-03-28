using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart

// ReSharper disable once CheckNamespace
namespace Azoth.Tools.Bootstrap.Compiler.IST.Classes
{
    using static Concrete;

    internal sealed partial class Package_Concrete : Package
    {
        public PackageSymbol Symbol { get; }
        public IFixedList<CompilationUnit_Concrete> CompilationUnits { get; }
        IFixedList<CompilationUnit> Package.CompilationUnits => CompilationUnits;

        public object? ImplementationRestricted => null;

        public Package_Concrete(PackageSymbol symbol, IFixedList<CompilationUnit_Concrete> compilationUnits)
        {
            Symbol = symbol;
            CompilationUnits = compilationUnits;
        }
    }

    internal sealed partial class CompilationUnit_Concrete : CompilationUnit
    {
        public CodeFile File { get; }
        public NamespaceName ImplicitNamespaceName { get; }

        public object? ImplementationRestricted => null;

        public CompilationUnit_Concrete(CodeFile file, NamespaceName implicitNamespaceName)
        {
            File = file;
            ImplicitNamespaceName = implicitNamespaceName;
        }
    }

    internal sealed partial class BoolLiteral_Concrete : BoolLiteral
    {
        public bool Value { get; }

        public object? ImplementationRestricted => null;

        public BoolLiteral_Concrete(bool value)
        {
            Value = value;
        }
    }

}

