using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public interface IMemberModel
{
    public static IEqualityComparer<IMemberModel> NameAndTypeComparer { get; }
        = EqualityComparer<IMemberModel>.Create((m1, m2) => m1?.Name == m2?.Name && m1?.Type == m2?.Type,
            m => HashCode.Combine(m.Name, m.Type));

    public static IEqualityComparer<IMemberModel> NameComparer { get; }
        = EqualityComparer<IMemberModel>.Create((m1, m2) => m1?.Name == m2?.Name,
            m => HashCode.Combine(m.Name));

    public static IEqualityComparer<IMemberModel> NameIsPlaceholderComparer { get; }
        = EqualityComparer<IMemberModel>.Create((m1, m2) => m1?.Name == m2?.Name && m1?.IsPlaceholder == m2?.IsPlaceholder,
            m => HashCode.Combine(m.Name, m.IsPlaceholder));

    TreeNodeModel Node { get; }
    string Name { get; }
    TypeModel Type { get; }
    TypeModel FinalType { get; }
    bool IsTemp { get; }
    bool IsPlaceholder { get; }
}
