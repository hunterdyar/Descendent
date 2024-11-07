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
            
            var psize = size/2-2 >= 0 ? size/2 - 2 : 0;
            var min = size / 2 - psize;
            var max = size / 2 + psize;
            Vector2Int pLoc = new Vector2Int(Random.Range(min, max), Random.Range(min, max));
            level.Agents.Add(pLoc, new Agent(AgentType.Player));
            return level;
        }
    }
}