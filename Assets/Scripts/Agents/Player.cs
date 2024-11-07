using UnityEngine;

namespace DefaultNamespace
{
    public class Player : GameAgentBase
    {
        public void Move(Vector2Int dir)
        {
            var c = CurrentPos;
            var test = Level.GetFirstAgentOrWall(c, dir);
            if (test.agent == null)
            {
                if (test.Item2 != c)
                {
                   MoveAgent(test.Item2);
                }
            }
            else
            {
                //hit the agent.
                
                //move to next to it.
                MoveAgent(test.Item2-dir);
            }
        }
    }
}