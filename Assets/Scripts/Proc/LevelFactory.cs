using System.Collections.Generic;
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
            List<(BSPNode node, ProtoLevel level)> rooms = new List<(BSPNode, ProtoLevel)>();
            root = BSPNode.Generate(depth, size,size);
            CreateAndAddProtosToNode(rl,root, ref rooms);
            
            //Create Connections between each split node.
            
            //pick one room to be starting and add the player to it. Pick a far away node and add exit to it.
            var startingRoom = rooms[Random.Range(0, rooms.Count)];
            rl.Agents.Add(startingRoom.level.PlayerStartLocation()+startingRoom.node.Position,AgentType.Player);
            
            
            return rl;
        }

        private static void CreateAndAddProtosToNode(RuntimeLevel rl, BSPNode node, ref List<(BSPNode, ProtoLevel)> rooms)
        {
            if(node.IsLeaf){
                if (node.Size.x > 1 && node.Size.y > 1)
                {
                    var pl = ProtoLevel.CreateRandomStampLevel(node.Size.x, node.Size.y);
                    rl.AddProtoLevel(pl, node.Position);
                    rooms.Add((node, pl));
                }
            }
            else
            {
                CreateAndAddProtosToNode(rl, node.ChildA, ref rooms);
                CreateAndAddProtosToNode(rl, node.ChildB, ref rooms);
            }

        }
    }
}