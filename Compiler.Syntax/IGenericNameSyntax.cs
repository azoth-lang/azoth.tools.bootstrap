using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

public partial interface IGenericNameSyntax
{
    public static IGenericNameSyntax Create(TextSpan span, string name, IFixedList<ITypeSyntax> typeArguments)
    {
        // TODO have a way to do this within the AG framework?
        var genericName = new GenericName(name, typeArguments.Count);
        return Create(span, genericName, typeArguments);
    }
}
