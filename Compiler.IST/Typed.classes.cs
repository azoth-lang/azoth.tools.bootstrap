using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace Azoth.Tools.Bootstrap.Compiler.IST.Classes
{
    using static Typed;

    internal sealed partial class IntLiteral_Typed : IntLiteral
    {
        public BigInteger Value { get; }
        public DataType Type { get; }

        public object? ImplementationRestricted => null;

        public IntLiteral_Typed(BigInteger value, DataType type)
        {
            Value = value;
        }
    }

}

namespace Azoth.Tools.Bootstrap.Compiler.IST.Classes
{
    using static Azoth.Tools.Bootstrap.Compiler.IST.Typed;

    internal sealed partial class Package_Concrete : Package
    {
        IFixedList<CompilationUnit> Package.CompilationUnits => CompilationUnits;
    }
    internal sealed partial class CompilationUnit_Concrete : CompilationUnit
    {
    }
}
