using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities
{
    /// <summary>
    /// Helper Class for extension methods for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class IndexedLinq
    {
        /// <summary>
        /// Adds an Index to an <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> on which to apply the indexing</param>
        /// <typeparam name="T">The <see cref="Type"/> of the elements in the <paramref name="source"/></typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> containing an <c>item</c> and an <c>index</c></returns>
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }
        
        /// <summary>
        /// Applies an <paramref name="action"/> to each element of the <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> on which we apply the provided <paramref name="action"/></param>
        /// <param name="action">The <see cref="Action{T}"/> to perform</param>
        /// <typeparam name="T">The <see cref="Type"/> of the elements in the <paramref name="source"/></typeparam>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach(T item in source)
                action(item);
        }
    }
}
    
