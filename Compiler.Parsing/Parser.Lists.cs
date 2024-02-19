using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    /// <summary>
    /// Accept items until not accepted
    /// </summary>
    public static IFixedList<T> AcceptMany<T>(Func<T?> acceptItem)
        where T : class
    {
        return new Generator<T?>(acceptItem)
               .TakeWhile(t => t is not null).ToFixedList()!;
    }

    /// <summary>
    /// Accept items as long as there is another separator
    /// </summary>
    public IFixedList<T> AcceptManySeparated<T, TSeparator>(Func<T?> acceptItem)
        where T : class
        where TSeparator : class, IToken
    {
        var items = new List<T>();
        var item = acceptItem();
        while (item is not null)
        {
            items.Add(item);
            if (!Tokens.Accept<TSeparator>()) break;

            item = acceptItem();
        }

        return items.ToFixedList();
    }

    /// <summary>
    /// Parse items until a terminator is found
    /// </summary>
    public IFixedList<T> ParseMany<T, TTerminator>(Func<T> parseItem)
        where T : class
        where TTerminator : IToken
    {
        var items = new List<T>();
        while (!Tokens.AtEnd<TTerminator>())
            items.Add(parseItem());

        return items.ToFixedList();
    }

    /// <summary>
    /// Parse an expected item and then parse additional items as long as there is
    /// a separator.
    /// </summary>
    public IFixedList<T> ParseManySeparated<T, TSeparator>(Func<T> parseItem)
        //where T : class
        where TSeparator : class, IToken
    {
        var item = parseItem();
        if (item is null) throw new ParseFailedException();
        var items = new List<T>() { item };

        while (Tokens.Accept<TSeparator>()) items.Add(parseItem());

        return items.ToFixedList();
    }

    /// <summary>
    /// Parse items as long as there is another separator or until a terminator is found
    /// </summary>
    public IFixedList<T> ParseManySeparated<T, TSeparator, TTerminator>(Func<T> parseItem)
        where T : class
        where TSeparator : class, IToken
        where TTerminator : IToken
    {
        var items = new List<T>();
        while (!Tokens.AtEnd<TTerminator>())
        {
            items.Add(parseItem());
            if (!Tokens.Accept<TSeparator>())
                break;
        }

        return items.ToFixedList();
    }
}
