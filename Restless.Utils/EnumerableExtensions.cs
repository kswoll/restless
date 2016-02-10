using System;
using System.Collections.Generic;
using System.Linq;

namespace Restless.Utils
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Compares `source` with `mergeWith`.  Items that are contained in `mergeWith` that are not contained in `source` 
        /// are placed in the `Added` property on the return value.  Items that are contained in `source` but not contained
        /// in `mergeWith` are placed in the `Removed` property on the return value.
        /// </summary>
        public static MergeResult<T, T, T> Merge<T>(this IEnumerable<T> source, IEnumerable<T> mergeWith)
        {
            return source.Merge(mergeWith, x => x, x => x, (x, y) => x);
        }

        public static MergeResult<TLeft, TRight, Tuple<TLeft, TRight>> Merge<TLeft, TRight, TKey>(this IEnumerable<TLeft> source, IEnumerable<TRight> mergeWith, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector)
        {
            return source.Merge(mergeWith, leftKeySelector, rightKeySelector, (x, y) => Tuple.Create(x, y));
        }

        public static MergeResult<TLeft, TRight, TUnchanged> Merge<TLeft, TRight, TUnchanged, TKey>(this IEnumerable<TLeft> source, IEnumerable<TRight> mergeWith, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<TLeft, TRight, TUnchanged> unchangedSelector)
        {
            var sourceSet = source.ToDictionary(x => leftKeySelector(x));
            var mergeWithKeys = mergeWith
                .Select(x => new { Item = x, Key = rightKeySelector(x) })
                .ToArray();
            Dictionary<TKey, TRight> mergeWithById;
            try
            {
                mergeWithById = mergeWithKeys
                    .Where(x => !EqualityComparer<TKey>.Default.Equals(x.Key, default(TKey)))
                    .ToDictionary(x => x.Key, x => x.Item);
            }
            catch (Exception)
            {
                var dups = mergeWithKeys.GroupBy(x => x.Key).Where(x => x.Count() > 1).ToArray();
                if (dups.Any())
                {
                    throw new Exception("Duplicate keys found: " + string.Join(", ", dups.Select(x => x.Key)));
                }
                else
                {
                    throw;
                }
            }
            var newMergeWith = mergeWithKeys.Where(x => EqualityComparer<TKey>.Default.Equals(x.Key, default(TKey))).Select(x => x.Item).ToArray();

            var removed = sourceSet.Where(item => !mergeWithById.Select(x => x.Key).Contains(item.Key)).Select(x => x.Value).ToList();
            var added = newMergeWith.Concat(mergeWithById.Where(item => !sourceSet.Select(x => x.Key).Contains(item.Key)).Select(x => x.Value)).ToList();
            var commonKeys = sourceSet.Select(x => x.Key).Intersect(mergeWithById.Select(x => x.Key)).ToArray();
            var unchanged = commonKeys.Select(x => unchangedSelector(sourceSet[x], mergeWithById[x])).ToList();

            return new MergeResult<TLeft, TRight, TUnchanged>(added, removed, unchanged);
        }
         
        public static IEnumerable<T> SelectRecursive<T>(this T obj, Func<T, T> next) where T : class
        {
            var current = obj;
            while (current != null)
            {
                yield return current;
                current = next(current);
            }
        }
    }
}