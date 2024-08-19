using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public static class MemberModelExtensions
{
    public static bool IsObjectMember(this IMemberModel model)
        => ObjectMembers.Contains(model.Name);

    private static readonly IFixedSet<string> ObjectMembers = new HashSet<string>()
    {
        "ToString", "GetHashCode", "Equals",
    }.ToFixedSet();

}
