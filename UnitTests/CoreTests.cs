using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnyTreeXPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class CoreTests
    {
        [TestMethod]
        public void CachedEnumerableEnumerate()
        {
            var source = Enumerable.Range(1, 5).ToList();

            var cachedEnumerable = new CachedEnumerable<int>(source);
            var result = new List<int>();
            foreach (var i in cachedEnumerable)
            {
                result.Add(i);
            }

            CollectionAssert.AreEqual(result, source);
        }

        [TestMethod]
        public void CachedEnumerableMultipleEnumerators()
        {
            var source = Enumerable.Range(1, 5).ToList();

            var cachedEnumerable = new CachedEnumerable<int>(source);

            var enumerator1 = cachedEnumerable.GetEnumerator();
            var result1 = new List<int>();
            enumerator1.MoveNext();
            result1.Add(enumerator1.Current);
            enumerator1.MoveNext();
            result1.Add(enumerator1.Current);

            var enumerator2 = cachedEnumerable.GetEnumerator();
            var result2 = new List<int>();
            enumerator2.MoveNext();
            result2.Add(enumerator2.Current);

            while (enumerator1.MoveNext())
            {
                result1.Add(enumerator1.Current);
            }
            while (enumerator2.MoveNext())
            {
                result2.Add(enumerator2.Current);
            }
            
            CollectionAssert.AreEqual(source, result1);
            CollectionAssert.AreEqual(source, result2);
        }
    }
}
