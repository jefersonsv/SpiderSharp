using System.Collections.Generic;
using System.Linq;

namespace SpiderSharp.Helpers
{
    public static class Collection
    {
        public static T GetLastN<T>(IEnumerable<T> ienumerable, int n)
        {
            return ienumerable.ElementAt(ienumerable.Count() - n);
        }

        public static IEnumerable<T> RemoveFirst<T>(IEnumerable<T> ienumerable)
        {
            var l = ienumerable.ToList();
            l.RemoveAt(0);
            return l;
        }
    }
}