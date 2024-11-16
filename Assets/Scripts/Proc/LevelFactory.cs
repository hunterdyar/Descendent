using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
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

            foreach (var room in rooms)
            {
                RoomData data = new RoomData(room.node.Position, room.node.Size);
                //todo: move to an AddRoom function in runtimelevel
                rl.Rooms.Add(data);
            }
            
            //pick one room to be starting and add the player to it. Pick a far away node and add exit to it.
            var startingRoom = rooms[Random.Range(0, rooms.Count)];
            rl.Agents.Add(startingRoom.level.PlayerStartLocation()+startingRoom.node.Position,AgentType.Player);
            
            
            return rl;
        }

        private static void CreateAndAddProtosToNode(RuntimeLevel rl, BSPNode node, ref List<(BSPNode, ProtoLevel)> rooms)
        {
            if(node.IsLeaf){
                if (node.Size.x >= 2 && node.Size.y >= 2)
                {
                   // int xg = node.SplitHorizontal ? 1 : 0;
                    //int yg = node.SplitHorizontal ? 0 : 1;
                    int xg=0, yg = 0;
                    var connectionPoints = new List<Vector2Int>();
                    GetConnectionPoints(node, ref connectionPoints);
                    connectionPoints = connectionPoints.Select(x => x - node.Position).ToList();
                   // var pl = ProtoLevel.CreateSolidLevel(node.Size.x - xg, node.Size.y - yg, connectionPoints);
                    var pl = ProtoLevel.CreateRandomStampLevel(node.Size.x - xg, node.Size.y - yg, connectionPoints);

                    //todo: debugging
                    //var pl = ProtoLevel.CreateRandomStampLevel(node.Size.x-xg, node.Size.y-yg);
                    rl.AddProtoLevel(pl, node.Position);
                    rooms.Add((node, pl));
                }
                else
                {
                    Debug.LogWarning("node size too small!");
                }
            }
            else
            {
                foreach (var internalConnectionPoint in node.InternalConnectionPoints)
                {
                    rl.Environment.Add(internalConnectionPoint, EnvTile.Floor);
                }
                CreateAndAddProtosToNode(rl, node.ChildA, ref rooms);
                CreateAndAddProtosToNode(rl, node.ChildB, ref rooms);
            }
        }

        /// <summary>
        /// recursively walk up the tree to get points. This can be cached and optimized or done at creation... but it's fine for now.
        /// </summary>

        private static void GetConnectionPoints(BSPNode node, ref List<Vector2Int> connectionPoints)
        {
            connectionPoints.Add(node.ConnectionPoint);
            if (node.Parent != null)
            {
                GetConnectionPoints(node.Parent, ref connectionPoints);
            }
        }
    }
}