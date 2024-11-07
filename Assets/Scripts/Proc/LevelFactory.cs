using UnityEngine;

namespace Proc
{
    public static class LevelFactory
    {
        public static Level CreateCircleLevel(int size)
        {
            if (size < 2)
            {
                return new Level();
            }
            Level level = new Level();
            float radius = (size / 2f)-0.5f;
            //make a circle
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    var pos = new Vector2(i, j) - new Vector2(radius, radius);
                    EnvTile t = pos.magnitude < radius ? EnvTile.Floor : EnvTile.Wall;
                    level.Environment.Add(new Vector2Int(i,j),t);
                }
            }
            
            //spawn player in the middle-ish i guess.
            var psize = size/2-2 >= 0 ? size/2 - 2 : 0;
            var min = size / 2 - psize;
            var max = size / 2 + psize;
            Vector2Int pLoc = new Vector2Int(Random.Range(min, max), Random.Range(min, max));
            level.Agents.Add(pLoc, AgentType.Player);
            
            //Some enemies
            int enemyCount = 5;
            for (int i = 0; i < enemyCount; i++)
            {
                var pos = level.GetRandomPosition(EnvTile.Floor);
                if (level.Agents.ContainsKey(pos))
                {
                    i--;
                    continue;
                }
                
                level.Agents.Add(pos, AgentType.Enemy);
            }
            
            return level;
            
        }
    }
}