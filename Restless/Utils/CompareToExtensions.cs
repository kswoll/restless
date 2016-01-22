using System;
using System.Collections.Generic;

namespace Restless.Utils
{
    public static class CompareToExtensions
    {
        public static void Sort<T, TValue>(this List<T> list, Func<T, TValue> selector)
            where TValue : ICompareTo<TValue>
        {
            list.Sort((x, y) =>
            {
                var xValue = selector(x);
                var yValue = selector(y);
                return xValue.IsThisPrimary(yValue) || !yValue.IsThisPrimary(xValue) ? xValue.CompareTo(yValue) : yValue.CompareTo(xValue);
            });
        }
    }
}