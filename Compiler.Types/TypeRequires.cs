using System.Diagnostics;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

internal static class TypeRequires
{
    [DebuggerHidden]
    public static void NoArgs<T>(IFixedList<T> arguments, string name)
        => Requires.That(arguments.IsEmpty, nameof(name), "Cannot have type arguments");
}
