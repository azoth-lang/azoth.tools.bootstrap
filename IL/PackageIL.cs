using System.ComponentModel.DataAnnotations;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.IL
{
    public class PackageIL
    {
        public FixedList<PackageReferenceIL> PackageReferences { get; }
        public FixedList<string> StringConstants { get; }
        public FixedList<string> SymbolConstants { get; }
        public FixedList<ClassIL> Classes { get; }
        public FixedList<FunctionIL> Functions { get; }
        public FixedList<DataType> Types { get; }
        public PackageIL(
            FixedList<string> stringConstants,
            FixedList<string> symbolConstants,
            FixedList<ClassIL> classes,
            FixedList<FunctionIL> functions,
            FixedList<DataType> types,
            FixedList<PackageReferenceIL> packageReferences)
        {
            StringConstants = stringConstants;
            SymbolConstants = symbolConstants;
            Classes = classes;
            Functions = functions;
            Types = types;
            PackageReferences = packageReferences;
        }
    }
}
