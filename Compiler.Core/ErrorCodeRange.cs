using System;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

public static class ErrorCodeRange
{
    public static readonly Range LexicalErrors = 1001..2000;
    public static readonly Range ParsingErrors = 2001..3000;
    public static readonly Range TypeErrors = 3001..4000;
    public static readonly Range FlowTypeErrors = 4001..5000;
    public static readonly Range NameBindingErrors = 5001..6000;
    public static readonly Range OtherSemanticErrors = 6001..7000;
}
