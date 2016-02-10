using System.Collections.Generic;

namespace Restless.Utils
{
    public struct MergeResult<TLeft, TRight, TUnchanged>
    {
        public List<TRight> Added { get; }
        public List<TLeft> Removed { get; }
        public List<TUnchanged> Unchanged { get; }

        public MergeResult(List<TRight> added, List<TLeft> removed, List<TUnchanged> unchanged)  : this()
        {
            Added = added;
            Removed = removed;
            Unchanged = unchanged;
        }
    }
}