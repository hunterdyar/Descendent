using UnityEngine;

namespace Proc
{
    public static class LevelFactory
    {
        public static RuntimeLevel CreateRandomValidSquareLevel(int size, int walls, int exits, int minMoves)
        {
            var pl = ProtoLevel.CreateRandomStampLevel(size, size);
            return RuntimeLevel.FromProtoLevel(pl, 0);
        }

        public static RuntimeLevel CreateDungeonLevels(int size, int depth, out BSPNode root)
        {
            var rl = new RuntimeLevel();
            root = BSPNode.Generate(depth, size,size);
            CreateAndAddProtosToNode(rl,root);
            return rl;
        }

        private static void CreateAndAddProtosToNode(RuntimeLevel rl, BSPNode node)
        {
            if(node.IsLeaf){
                var pl = ProtoLevel.CreateRandomStampLevel(node.Size.x, node.Size.y);
                rl.AddProtoLevel(pl,node.Position);
            }
            else
            {
                CreateAndAddProtosToNode(rl, node.ChildA);
                CreateAndAddProtosToNode(rl, node.ChildB);
            }
        }
    }
}