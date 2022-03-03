using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScript
{
    public static class FunctionUtils
    {
        public static TOut Apply<TIn, TOut>(this TIn @in, Func<TIn, TOut> func) => func(@in);
        
    }
    public static class IEnumUtils
    {
        public static List<T?> CastNullable<T>(this IEnumerable<T> self)
        {
            var res = new List<T?>();
            foreach (var item in self)
            {
                res.Add(item);
            }
            return res;
        }
        public static List<R> ConvertAll<T,R>(this IEnumerable<T> self, Func<T,R> func)
        {
            var res = new List<R>();
            foreach (var item in self)
            {
                res.Add(func(item));
            }
            return res;
        }
    }
}
