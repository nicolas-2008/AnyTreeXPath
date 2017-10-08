using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AnyTreeXPath
{
    public class CachedEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _enumerable;
        private readonly IEnumerator<T> _enumerator;
        private readonly List<T> _cache = new List<T>();
        private bool _done;

        public CachedEnumerable(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
            _enumerator = _enumerable.GetEnumerator();
        }

        public int Count
        {
            get
            {
                if (!_done)
                {
                    ReadUntil(int.MaxValue);
                }

                return _cache.Count;
            }
        }

        public T this[int i]
        {
            get
            {
                ReadUntil(i);
                return _cache[i];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            // get snapshot of current state as it may be modified (by calling Count or indexer) during enumeration
            var itemsSnapshot = _cache.ToList();
            var doneShapshot = _done;

            foreach (var item in itemsSnapshot)
            {
                yield return item;
            }

            if (!doneShapshot)
            {
                int index = itemsSnapshot.Count;
                while (ReadUntil(index))
                {
                    yield return _cache[index++];
                }

                _done = true;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ReadUntil(int i)
        {
            int count = _cache.Count;
            while (!_done && count <= i)
            {
                if (_enumerator.MoveNext())
                {
                    _cache.Add(_enumerator.Current);
                    ++count;
                }
                else
                {
                    _done = true;
                }
            }

            return count > i;
        }
    }
}