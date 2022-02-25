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
}
