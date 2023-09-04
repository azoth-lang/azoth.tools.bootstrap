using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.Framework
{
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    [DebuggerTypeProxy(typeof(DictionaryDebugView<,>))]
    public class FixedDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
        where TKey : notnull
    {
        public static readonly FixedDictionary<TKey, TValue> Empty = new(new Dictionary<TKey, TValue>());

        private readonly IReadOnlyDictionary<TKey, TValue> items;

        [DebuggerStepThrough]
        public FixedDictionary(IDictionary<TKey, TValue> items)
        {
            this.items = new ReadOnlyDictionary<TKey, TValue>(new Dictionary<TKey, TValue>(items));
        }

        [DebuggerStepThrough]
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            => items.GetEnumerator();

        [DebuggerStepThrough]
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)items).GetEnumerator();

        public int Count
        {
            [DebuggerStepThrough]
            get => items.Count;
        }

        [DebuggerStepThrough]
        public bool ContainsKey(TKey key) => items.ContainsKey(key);

        [DebuggerStepThrough]
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
            => items.TryGetValue(key, out value);

        public TValue this[TKey key]
        {
            [DebuggerStepThrough]
            get => items[key];
        }

        public IEnumerable<TKey> Keys
        {
            [DebuggerStepThrough]
            get => items.Keys;
        }

        public IEnumerable<TValue> Values
        {
            [DebuggerStepThrough]
            get => items.Values;
        }
    }
}
