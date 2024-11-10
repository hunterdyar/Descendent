using UnityEngine;

namespace Proc
{
    public static class LevelFactory
    {
        public static RuntimeLevel CreateRandomValidSquareLevel(int size, int walls, int exits, int minMoves)
        {
            for (int i = 0; i < 1000; i++)
            {
                var pl = ProtoLevel.CreateRandomStampLevel(size, size);
                if (Solver.Solve(pl, minMoves))
                {
                    return RuntimeLevel.FromProtoLevel(pl, Random.Range(1,5));
                }
            }
            Debug.LogError("Unable to create random valid level.");
            return null;
        }
        
        public static RuntimeLevel CreateCircleLevel(int size)
        {
            if (size < 2)
            {
                return new RuntimeLevel();
            }
            RuntimeLevel runtimeLevel = new RuntimeLevel();
            float radius = (size / 2f)-0.5f;
            //make a circle
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    var pos = new Vector2(i, j) - new Vector2(radius, radius);
                    EnvTile t = pos.magnitude < radius ? EnvTile.Floor : EnvTile.Wall;
                    runtimeLevel.Environment.Add(new Vector2Int(i,j),t);
                }
            }
            
            //spawn player in the middle-ish i guess.
            var psize = size/2-2 >= 0 ? size/2 - 2 : 0;
            var min = size / 2 - psize;
            var max = size / 2 + psize;
            Vector2Int pLoc = new Vector2Int(Random.Range(min, max), Random.Range(min, max));
            runtimeLevel.Agents.Add(pLoc, AgentType.Player);
            
            //Some enemies
            int enemyCount = 5;
            for (int i = 0; i < enemyCount; i++)
            {
                var pos = runtimeLevel.GetRandomPosition(EnvTile.Floor);
                if (runtimeLevel.Agents.ContainsKey(pos))
                {
                    i--;
                    continue;
                }
                
                runtimeLevel.Agents.Add(pos, AgentType.Enemy);
            }
            
            return runtimeLevel;
            
        }
    }
}