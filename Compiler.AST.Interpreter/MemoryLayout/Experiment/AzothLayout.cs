using System;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout.Experiment;

internal partial class AzothLayout
{
    public static IAzothObject CreateObject(VTable vTable, int references, int ints, int bytes)
    {
        var objectType = AzothObjectType.MakeGenericType(ReferencesTypes[references], IntsTypes[ints], BytesTypes[bytes]);
        return (IAzothObject?)Activator.CreateInstance(objectType, vTable) ?? throw new UnreachableCodeException();
    }

    private static readonly Type AzothObjectType = typeof(AzothObject<,,>);
}
