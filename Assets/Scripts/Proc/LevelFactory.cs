using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;

namespace Proc
{
    public static class LevelFactory
    {
        public static RuntimeLevel CreateDungeon(int size, int depth, out BSPNode root, out PlayerGraph playerGraph)
        {
            root = BSPNode.Generate(depth, size,size);
            var pl = new ProtoLevel(root);
            pl.Generate();//level generation stats?
            
            playerGraph = pl.GetPlayerGraph();
            return RuntimeLevel.FromProtoLevel(pl, 0);
        }
    }
}