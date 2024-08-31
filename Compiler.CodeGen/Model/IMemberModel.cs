using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public interface IMemberModel
{
    public static IEqualityComparer<IMemberModel> NameAndTypeComparer { get; }
        = EqualityComparer<IMemberModel>.Create((p1, p2) => p1?.Name == p2?.Name && p1?.Type == p2?.Type,
            p => HashCode.Combine(p.Name, p.Type));

    public static IEqualityComparer<IMemberModel> NameComparer { get; }
        = EqualityComparer<IMemberModel>.Create((p1, p2) => p1?.Name == p2?.Name, p => HashCode.Combine(p.Name));

    TreeNodeModel Node { get; }
    string Name { get; }
    TypeModel Type { get; }
    TypeModel FinalType { get; }
    bool IsTemp { get; }
}
