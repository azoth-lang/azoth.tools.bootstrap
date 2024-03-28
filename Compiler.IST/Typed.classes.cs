using System.CodeDom.Compiler;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart

// ReSharper disable once CheckNamespace
namespace Azoth.Tools.Bootstrap.Compiler.IST.Classes
{
    using static Typed;

    [GeneratedCode("AzothCompilerCodeGen", null)]
    internal sealed partial class IntLiteral_Typed : IntLiteral
    {
        public BigInteger Value { get; }
        public DataType Type { get; }

        public object? ImplementationRestricted => null;

        public IntLiteral_Typed(BigInteger value, DataType type)
        {
            Value = value;
            Type = type;
        }
    }

    [GeneratedCode("AzothCompilerCodeGen", null)]
    internal sealed partial class StringLiteral_Typed : StringLiteral
    {
        public string Value { get; }
        public DataType Type { get; }

        public object? ImplementationRestricted => null;

        public StringLiteral_Typed(string value, DataType type)
        {
            Value = value;
            Type = type;
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
