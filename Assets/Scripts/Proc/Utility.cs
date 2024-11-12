using System.Collections.Generic;
using System.Linq;

namespace Proc
{
    public static class Utility
    {
        private static PDir[][] RandomDirections;
        private static bool _calculatedDirections;
        private static int _perms;
        
        public static PDir[] GetDirectionsShuffled()
        {
            if (!_calculatedDirections)
            {
                _calculatedDirections = true;
                RandomDirections = GetPermutations(ProtoLevel.Directions,4).Select(x=>x.ToArray()).ToArray();
                _perms = RandomDirections.Length;
            }
            return RandomDirections[UnityEngine.Random.Range(0, _perms)];
        }
        static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(o => !t.Contains(o)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}