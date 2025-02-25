using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Framework.Collections;

[StructLayout(LayoutKind.Auto)]
internal readonly record struct ItemData<TItemData>(TItemData Data, int SetIndex);
