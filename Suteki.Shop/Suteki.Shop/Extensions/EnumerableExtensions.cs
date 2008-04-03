using System;
using System.Collections.Generic;

namespace Suteki.Shop.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Use instead of a foreach loop e.g.
        /// MyCollection.Each(item => DoSomething(item));
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="function"></param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T item in items)
            {
                action(item);
            }
        }

        /// <summary>
        /// Convenient replacement for a range 'for' loop. e.g. return an array of int from 10 to 20:
        /// int[] tenToTwenty = 10.to(20).ToArray();
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IEnumerable<int> To(this int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                yield return i;
            }
        }
    }
}
