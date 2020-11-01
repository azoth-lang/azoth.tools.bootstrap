using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Lexing
{
    public interface ITokenIterator<out TToken>
        where TToken : class, IToken
    {
        ParseContext Context { get; }

        bool Next();

        /// <exception cref="InvalidOperationException">If current is accessed after Next() has returned false</exception>
        TToken Current { get; }
    }
}
