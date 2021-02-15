using System.Collections;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.IR.Declarations;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IR
{
    /// <summary>
    /// A table of entities as used in IR
    /// </summary>
    public class Table<T> : IEnumerable<T>
        where T : notnull
    {
        private readonly List<T> values = new List<T>();
        private readonly Dictionary<T, uint> lookup = new Dictionary<T, uint>();

        public uint this[T value] => lookup[value];

        public uint GetOrAdd(T value)
        {
            if (lookup.TryGetValue(value, out var index)) return index;

            index = (uint)values.Count;
            values.Add(value);
            lookup.Add(value, index);
            return index;
        }

        public FixedList<T> ToFixedList()
        {
            return values.ToFixedList();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
