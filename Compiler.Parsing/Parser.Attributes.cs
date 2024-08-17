using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    private IFixedList<IAttributeSyntax> ParseAttributes()
        => AcceptMany(AcceptAttribute);

    private IAttributeSyntax? AcceptAttribute()
    {
        var accept = Tokens.AcceptToken<IHashToken>();
        if (accept is null) return null;
        var typeName = ParseStandardTypeName();
        var span = TextSpan.Covering(accept.Span, typeName.Span);
        return IAttributeSyntax.Create(span, typeName);
    }
}
