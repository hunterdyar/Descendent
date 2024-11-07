using UnityEngine;

namespace DefaultNamespace
{
    public class Player : Pawn
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
                var agentWeHit = test.agent;
                
                //move to next to it.
                if (test.Item2-dir != c)
                {
                    MoveAgent(test.Item2 - dir);
                }

                //FIGHT
                if (agentWeHit is Pawn pawn)
                {
                    var duel = new Duel(this, pawn);
                    duel.Resolve();
                }
            }
        }
    }
}