using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

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
        
        public static PDir GetRandomOrthogonalDirection(PDir dir)
        {
            return dir switch
            {
                PDir.Up or PDir.Down => Random.value < 0.5f ? PDir.Left : PDir.Right,
                PDir.Left or PDir.Right => Random.value < 0.5f ? PDir.Up : PDir.Down,
                _ => throw new Exception("Invalid Direction")
            };
        }
        
        public static bool IsOppositeDir(PDir a, PDir b)
        {
            switch (a)
            {
                case PDir.Up:
                    return b == PDir.Down;
                case PDir.Down :
                    return b == PDir.Up;
                case PDir.Left:
                    return b == PDir.Right;
                case PDir.Right:
                    return b == PDir.Left;
            }
            throw new Exception("Invalid Direction");
        }

        public static (int x, int y) PDirToXY(PDir dir)
        {
            switch (dir)
            {
                case PDir.Down:
                    return (0, -1);
                case PDir.Up:
                    return (0, 1);
                case PDir.Left:
                    return (-1, 0);
                case PDir.Right:
                    return (1, 0);
                default:
                    throw new Exception("Invalid PDir");
            }
        }
    }
}