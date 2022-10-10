using System.Collections.Generic;
using System.Linq;

namespace Utilities
{
    public static class IndexedLinq
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }
    }
}
    
