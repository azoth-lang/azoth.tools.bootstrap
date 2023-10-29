using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    private FixedList<IAttributeSyntax> ParseAttributes()
        => AcceptMany(AcceptAttribute);

    private IAttributeSyntax? AcceptAttribute()
    {
        var accept = Tokens.AcceptToken<IHashToken>();
        if (accept is null) return null;
        var typeName = ParseTypeName();
        var span = TextSpan.Covering(accept.Span, typeName.Span);
        return new AttributeSyntax(span, typeName);
    }
}
