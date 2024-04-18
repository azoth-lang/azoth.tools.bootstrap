using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.IST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;

internal sealed partial class SyntaxBinder
{
    private IEnumerable<Concrete.GenericParameter> Transform(IEnumerable<IGenericParameterSyntax> from)
        => from.Select(Transform).ToFixedSet();

    private partial Concrete.GenericParameter Transform(IGenericParameterSyntax from);

    private IEnumerable<Concrete.UnresolvedSupertypeName> Transform(IEnumerable<ISupertypeNameSyntax> from)
        => from.Select(f => Transform(f)).ToFixedSet();

    [return: NotNullIfNotNull(nameof(from))]
    private partial Concrete.UnresolvedSupertypeName? Transform(ISupertypeNameSyntax? from);

    private IEnumerable<Concrete.UnresolvedType> Transform(IEnumerable<ITypeSyntax> from)
        => from.Select(Transform).ToFixedSet();

    private partial Concrete.UnresolvedType Transform(ITypeSyntax from);
}
